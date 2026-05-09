using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Email;
using eVote470Plus.Core.Application.Interfaces.Email;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.ViewModels.Relations.Vote;
using eVote470Plus.Core.Application.ViewModels.VoterUI;
using eVote470Plus.Core.Application.ViewModels.VoteScreen;
using eVote470Plus.Core.Domain.Common.Enum;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    public class VotingController : Controller
    {
        private readonly ICitizenService _citizenService;
        private readonly IElectionService _electionService;
        private readonly IVoteService _voteService;
        private readonly IPoliticalPositionService _politicalPositionService;
        private readonly ICandidatePositionService _candidatePositionService;
        private readonly ICandidateService _candidateService;
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public VotingController(
            ICitizenService citizenService,
            IElectionService electionService,
            IVoteService voteService,
            IPoliticalPositionService politicalPositionService,
            ICandidatePositionService candidatePositionService,
            ICandidateService candidateService,
            IPoliticalPartyService politicalPartyService,
            IEmailService emailService,
            IMapper mapper)
        {
            _citizenService = citizenService;
            _electionService = electionService;
            _voteService = voteService;
            _politicalPositionService = politicalPositionService;
            _candidatePositionService = candidatePositionService;
            _candidateService = candidateService;
            _politicalPartyService = politicalPartyService;
            _emailService = emailService;
            _mapper = mapper;
        }

        // Index (Document entry) 

        public IActionResult Index()
        {
            // Admins and leaders should not access voting
            if (User.IsInRole(nameof(Roles.Administrator)))
                return RedirectToAction("Index", "AdminHome");

            if (User.IsInRole(nameof(Roles.PoliticalLeader)))
                return RedirectToAction("Index", "LeaderHome");

            HttpContext.Session.Remove("ValidatedVoterId");

            return View(new VoterIdentityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VoterIdentityViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Check active election exists
            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
            {
                ModelState.AddModelError(string.Empty, "There is no electoral process at the moment.");
                return View(vm);
            }

            // Find citizen by document number
            var citizen = await _citizenService.GetByDocumentNumberAsync(vm.DocumentNumber);
            if (citizen == null)
            {
                ModelState.AddModelError(nameof(vm.DocumentNumber), "There is no registered citizen with this document number.");
                return View(vm);
            }

            // Check citizen is active
            if (!citizen.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This citizen is inactive and cannot participate in the electoral process.");
                return View(vm);
            }

            // Check if citizen has already voted in this election
            bool hasCompleted = await _voteService.HasCompletedVotingAsync(citizen.CitizenId, activeElection.ElectionId);
            if (hasCompleted)
            {
                ModelState.AddModelError(string.Empty, "You have already exercised your right to vote.");
                return View(vm);
            }

            // Redirect to OCR identity validation
            return RedirectToAction(nameof(ValidateIdentity), new { documentNumber = vm.DocumentNumber });
        }

        // ValidateIdentity (OCR) 

        public IActionResult ValidateIdentity(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return RedirectToAction(nameof(Index));

            return View(new VoterOCRViewModel { DocumentNumber = documentNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateIdentity(VoterOCRViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Process image with Tesseract OCR
            string extractedText = await ExtractTextFromImageAsync(vm.DocumentPhoto);
            string extractedDocument = ExtractDocumentNumber(extractedText);

            // Compare extracted document with the one entered by the citizen
            if (!extractedDocument.Equals(vm.DocumentNumber, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "The data extracted from the photo does not match the previously entered data.";
                return View(vm);
            }

            // Get citizen and redirect to positions
            var citizen = await _citizenService.GetByDocumentNumberAsync(vm.DocumentNumber);

            HttpContext.Session.SetInt32("ValidatedVoterId", citizen!.CitizenId);

            return RedirectToAction(nameof(Positions), new { citizenId = citizen.CitizenId });
        }

        // List of available positions

        public async Task<IActionResult> Positions(int citizenId)
        {
            if (!IsValidatedVoter(citizenId))
                return RedirectToAction(nameof(Index));

            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
                return RedirectToAction(nameof(Index));

            var citizen = await _citizenService.GetByIdAsync(citizenId);
            if (citizen == null)
                return RedirectToAction(nameof(Index));

            var activePositions = await _politicalPositionService.GetActivePositionsAsync();
            var votedPositionIds = await _voteService.GetVotedPositionIdsAsync(citizenId, activeElection.ElectionId);

            var vms = new List<VotingPositionViewModel>();

            foreach (var position in activePositions)
            {
                // Get all candidate assignments for this position
                var assignments = await _candidatePositionService.GetByPositionAsync(position.PoliticalPositionId);

                // Count unique candidates and parties for this position
                var uniqueCandidateIds = assignments.Select(a => a.CandidateId).Distinct().ToList();
                var uniquePartyIds = assignments.Select(a => a.PoliticalPartyId).Distinct().ToList();

                vms.Add(new VotingPositionViewModel
                {
                    PoliticalPositionId = position.PoliticalPositionId,
                    Name = position.Name,
                    CandidateCount = uniqueCandidateIds.Count,
                    PartyCount = uniquePartyIds.Count,
                    AlreadyVoted = votedPositionIds.Contains(position.PoliticalPositionId)
                });
            }

            // Check if all positions have been voted and show pending message if any
            var pendingPositions = vms.Where(p => !p.AlreadyVoted).ToList();
            if (pendingPositions.Any() && TempData["PendingPositions"] != null)
            {
                ViewBag.PendingMessage = $"You still need to select a candidate for: {string.Join(", ", pendingPositions.Select(p => p.Name))}.";
            }

            ViewBag.CitizenId = citizenId;
            ViewBag.ElectionId = activeElection.ElectionId;
            ViewBag.AllVoted = !pendingPositions.Any();
            return View(vms);
        }

        // Vote (Ballot for a specific position) 

        public async Task<IActionResult> Vote(int citizenId, int positionId)
        {
            if (!IsValidatedVoter(citizenId))
                return RedirectToAction(nameof(Index));

            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
                return RedirectToAction(nameof(Index));

            // Check citizen hasn't already voted for this position
            var votedPositionIds = await _voteService.GetVotedPositionIdsAsync(citizenId, activeElection.ElectionId);
            if (votedPositionIds.Contains(positionId))
            {
                TempData["Error"] = "You already voted for this position.";
                return RedirectToAction(nameof(Positions), new { citizenId });
            }

            var position = await _politicalPositionService.GetByIdAsync(positionId);
            if (position == null)
                return RedirectToAction(nameof(Positions), new { citizenId });

            // Get all candidate assignments for this position across all parties
            var allAssignments = await _candidatePositionService.GetAllAsync();
            var positionAssignments = allAssignments
                .Where(a => a.PoliticalPositionId == positionId)
                .ToList();

            var candidateOptions = new List<CandidateOptionViewModel>();

            foreach (var assignment in positionAssignments)
            {
                var candidate = await _candidateService.GetByIdAsync(assignment.CandidateId);
                var party = await _politicalPartyService.GetByIdAsync(assignment.PoliticalPartyId);

                if (candidate == null || !candidate.IsActive) continue;
                if (party == null || !party.IsActive) continue;

                candidateOptions.Add(new CandidateOptionViewModel
                {
                    CandidateId = candidate.CandidateId,
                    Name = candidate.Name,
                    LastName = candidate.LastName,
                    PhotoUrl = candidate.PhotoUrl,
                    PartyName = party.Name,
                    PartyLogoUrl = party.LogoUrl
                });
            }

            // Add "None" option 
            candidateOptions.Add(new CandidateOptionViewModel
            {
                CandidateId = 0,
                Name = "None",
                LastName = string.Empty,
                PhotoUrl = null,
                PartyName = string.Empty,
                PartyLogoUrl = null
            });

            var vm = new CandidateSelectionViewModel
            {
                PoliticalPositionId = positionId,
                PoliticalPositionName = position.Name,
                Candidates = candidateOptions
            };

            ViewBag.CitizenId = citizenId;
            ViewBag.ElectionId = activeElection.ElectionId;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(CandidateSelectionViewModel vm, int citizenId, int electionId)
        {
            if (!IsValidatedVoter(citizenId))
                return RedirectToAction(nameof(Index));

            // Revalidate active election instead of trusting client data
            var activeElection = await _electionService.GetActiveElectionAsync();

            if (activeElection == null)
                return RedirectToAction(nameof(Index));

            ModelState.Remove(nameof(vm.Candidates));
            ModelState.Remove(nameof(vm.PoliticalPositionName));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "You must select a candidate before voting.";

                return RedirectToAction(nameof(Vote), new
                {
                    citizenId,
                    positionId = vm.PoliticalPositionId
                });
            }

            // Cast the vote using server-side election id
            await _voteService.CastVoteAsync(
                citizenId,
                activeElection.ElectionId,
                vm.SelectedCandidateId,
                vm.PoliticalPositionId
            );

            // Return to positions list
            return RedirectToAction(nameof(Positions), new { citizenId });
        }

        // Finish 

        public async Task<IActionResult> Finish(int citizenId)
        {
            if (!IsValidatedVoter(citizenId))
                return RedirectToAction(nameof(Index));

            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
                return RedirectToAction(nameof(Index));

            var activePositions = await _politicalPositionService.GetActivePositionsAsync();
            var votedPositionIds = await _voteService.GetVotedPositionIdsAsync(citizenId, activeElection.ElectionId);

            // Validate all positions have been voted
            var pendingPositions = activePositions
                .Where(p => !votedPositionIds.Contains(p.PoliticalPositionId))
                .ToList();

            if (pendingPositions.Any())
            {
                TempData["PendingPositions"] = true;
                return RedirectToAction(nameof(Positions), new { citizenId });
            }

            // All positions voted, send summary email
            var citizen = await _citizenService.GetByIdAsync(citizenId);
            if (citizen != null && !string.IsNullOrWhiteSpace(citizen.Email))
            {
                string htmlBody = await BuildVotingSummaryEmailAsync(citizenId, activeElection.ElectionId);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    ToEmail = citizen.Email,
                    Subject = $"Summary of your vote - {activeElection.Name}",
                    HtmlBody = htmlBody
                });
            }

            HttpContext.Session.Remove("ValidatedVoterId");

            TempData["Success"] = "You have successfully completed your voting process!";
            return RedirectToAction(nameof(Index));
        }

        // OCR Helpers 

        private async Task<string> ExtractTextFromImageAsync(IFormFile file)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(stream);

            try
            {
                using var engine = new Tesseract.TesseractEngine(@"./tessdata", "spa", Tesseract.EngineMode.Default);
                using var img = Tesseract.Pix.LoadFromFile(tempPath);
                using var page = engine.Process(img);
                return page.GetText();
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }

        private string ExtractDocumentNumber(string ocrText)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                ocrText,
                @"\d{3}-?\d{7}-?\d{1}"
            );

            if (match.Success)
                return match.Value.Replace("-", "");

            var fallback = System.Text.RegularExpressions.Regex.Match(ocrText, @"\d{11}");
            return fallback.Success ? fallback.Value : string.Empty;
        }

        // Email Helper

        private async Task<string> BuildVotingSummaryEmailAsync(int citizenId, int electionId)
        {
            var votes = await _voteService.GetVotedPositionIdsAsync(citizenId, electionId);
            var html = "<h2>Summary of your vote</h2><ul>";

            foreach (var positionId in votes)
            {
                var position = await _politicalPositionService.GetByIdAsync(positionId);
                var allVotes = await _voteService.GetVotesByPositionAndElectionAsync(positionId, electionId);
                var citizenVote = allVotes.FirstOrDefault(v => v.CitizenId == citizenId);

                string candidateName = "None";
                if (citizenVote != null && citizenVote.CandidateId != 0)
                {
                    var candidate = await _candidateService.GetByIdAsync(citizenVote.CandidateId);
                    candidateName = candidate != null ? $"{candidate.Name} {candidate.LastName}" : "None";
                }

                html += $"<li><strong>{position?.Name}</strong>: {candidateName}</li>";
            }

            html += "</ul>";
            return html;
        }

        private bool IsValidatedVoter(int citizenId)
        {
            var validatedId = HttpContext.Session.GetInt32("ValidatedVoterId");

            return validatedId.HasValue && validatedId.Value == citizenId;
        }
    }
}