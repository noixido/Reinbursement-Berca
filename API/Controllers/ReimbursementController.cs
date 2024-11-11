using API.Models;
using API.Repositories;
using API.Repositories.Interface;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReimbursementController : ControllerBase
    {
        private ReimbursementRepositories _reimbursementRepositories;

        public ReimbursementController(ReimbursementRepositories reimbursementRepositories)
        {
            _reimbursementRepositories = reimbursementRepositories;
        }

        [HttpPost]
        public IActionResult addReimbursement(ReimbursementVM reimbursement)
        {
            if (String.IsNullOrWhiteSpace(reimbursement.Id_Account))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"ID Account harus diisi",
                });
            }
            else if (String.IsNullOrWhiteSpace(reimbursement.Id_Category))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"ID Kategori harus diisi",
                });
            }
            else if (String.IsNullOrWhiteSpace(reimbursement.Evidence))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Evidence harus diisi",
                });
            }
            else if (reimbursement.Amount == null || reimbursement.Amount == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Amount harus diisi",
                });
            }
            else if (reimbursement.Submit_Date == default(DateTime))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Submit date harus diisi",
                });
            }

            var addReimbursement = _reimbursementRepositories.AddReimbursement(reimbursement);

            if (addReimbursement <= 0)  
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Data gagal ditambahkan",
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = $"Data berhasil ditambahkan",
                Data = reimbursement
            });
        }

        [HttpGet("account/{accountId}")]
        public IActionResult GetAllReimbursementByAccount(string accountId)
        {
            //NANTI DITAMBAHIN PENGECEKAN APAKAH ACCOUNT ADA ATAU TIDAK


            var reimbursements = _reimbursementRepositories.GetAllReimbursementByAccount(accountId);

            if (reimbursements.Count() == 0)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Data tidak ditemukan"
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = $"{reimbursements.Count()} data ditemukan",
                Data = reimbursements
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetReimbursementById(string id)
        {
            //NANTI DITAMBAHIN PENGECEKAN APAKAH ACCOUNT ADA ATAU TIDAK


            var reimbursement = _reimbursementRepositories.GetReimbursementById(id);

            if (reimbursement == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Data {id} tidak ditemukan"
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = $"Data {reimbursement.Id_Reimbursement} berhasil ditemukan",
                Data = reimbursement
            });

        }

        [HttpGet]
        public IActionResult GetAllReimbursements()
        {
            var reimbursements = _reimbursementRepositories.GetAllReimbursements();

            if (reimbursements.Count() == 0)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Data tidak ditemukan"
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = $"{reimbursements.Count()} data ditemukan",
                Data = reimbursements
            });
        }

        [HttpPut("hr/approve/{id}")]
        public IActionResult ApproveReimbursementByHR(string id, [FromQuery] string note = "Approve reimbursement by HR")
        {
            var result = _reimbursementRepositories.UpdateReimbursementStatusByHR(id, "Approved", note);

            if (result == -1)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Data {id} tidak ditemukan"
                });
            }
            else if (result <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gagal mengupdate status reimbursement"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reimbursement berhasil di-approve oleh HR",
                Note = note
            });
        }

        [HttpPut("hr/decline/{id}")]
        public IActionResult DeclineReimbursementByHR(string id, [FromQuery] string note = "Decline reimbursement by HR")
        {
            var result = _reimbursementRepositories.UpdateReimbursementStatusByHR(id, "Declined", note);

            if (result == -1)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Data {id} tidak ditemukan"
                });
            }
            else if (result <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gagal mengupdate status reimbursement"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reimbursement berhasil ditolak oleh HR",
                Note = note
            });
        }

        [HttpPut("finance/approve/{id}")]
        public IActionResult ApproveReimbursementByFinance(string id, float Approve_Amount, [FromQuery] string note = "Approve reimbursement by Finance")
        {
            var result = _reimbursementRepositories.UpdateReimbursementStatusByFinance(id, "Approved by Finance", note, Approve_Amount);

            if (result == -1)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Data {id} tidak ditemukan"
                });
            }
            else if (result <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gagal mengupdate status reimbursement"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reimbursement berhasil di-approve oleh Finance",
                ApprovedAmount = Approve_Amount,
                Note = note
            });
        }

        [HttpPut("finance/decline/{id}")]
        public IActionResult DeclineReimbursementByFinance(string id, [FromQuery] string note = "Decline reimbursement by Finance")
        {
            var result = _reimbursementRepositories.UpdateReimbursementStatusByFinance(id, "Declined by Finance", note, 0);

            if (result == -1)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Data {id} tidak ditemukan"
                });
            }
            else if (result <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gagal mengupdate status reimbursement"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reimbursement berhasil ditolak oleh Finance",
                Note = note
            });
        }

    }
}
