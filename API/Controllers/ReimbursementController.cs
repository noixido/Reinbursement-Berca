using API.Models;
using API.Repositories;
using API.Repositories.Interface;
using API.ViewModels;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [Authorize(Roles = "HR, Employee, Finance")]
    public class ReimbursementController : ControllerBase
    {
        private ReimbursementRepositories _reimbursementRepositories;

        public ReimbursementController(ReimbursementRepositories reimbursementRepositories)
        {
            _reimbursementRepositories = reimbursementRepositories;
        }

        [HttpPost]
        public async Task<IActionResult> addReimbursement([FromForm] ReimbursementRequest reimbursement)
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
            else if (reimbursement.Evidence == null || reimbursement.Evidence.Length == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Evidence harus diisi",
                });
            }

            // Membuat nama file unik dengan UUID
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(reimbursement.Evidence.FileName)}";
            // Menambahkan Id_Account ke dalam path
            var folderPath = Path.Combine("wwwroot/uploads/evidence", reimbursement.Id_Account);

            // Membuat folder jika belum ada
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Path file akhir
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // Menyimpan file ke disk
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await reimbursement.Evidence.CopyToAsync(stream);
            }

            // Mapping model ke ReimbursementVM sebelum mengirim ke repository
            var newReimbursement = new ReimbursementVM
            {
                Id_Account = reimbursement.Id_Account,
                Id_Category = reimbursement.Id_Category,
                Evidence = Path.Combine("uploads", "evidence", reimbursement.Id_Account, uniqueFileName).Replace("\\", "/"),
                Amount = reimbursement.Amount,
                Submit_Date = reimbursement.Submit_Date
            };

            var addReimbursement = await _reimbursementRepositories.AddReimbursement(newReimbursement);

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

        [HttpGet("hr")]
        public IActionResult GetAllReimbursementsHR()
        {
            var reimbursements = _reimbursementRepositories.GetAllReimbursementsByStatus("Progress in HR");

            if (reimbursements.Count() == 0)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data tidak ditemukan",
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = $"{reimbursements.Count()} data ditemukan",
                Data = reimbursements
            });
        }

        [HttpGet("finance")]
        public IActionResult GetAllReimbursementsFinance()
        {
            var reimbursements = _reimbursementRepositories.GetAllReimbursementsByStatus("Progress in Finance");

            if (reimbursements.Count() == 0)
            {
                return Ok(new
                {
                    StatusCode = 200,
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

        [HttpPut("approvehr/{id}")]
        public IActionResult ApproveReimbursementByHR(string id, ChangeStatusReimbursement changeReimbursement)
        {
            bool isApprove = true;
            var result = _reimbursementRepositories.UpdateReimbursementStatusByHR(id, changeReimbursement, isApprove);

            if (result == ReimbursementRepositories.NOTFOUND)
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

                Message = "Reimbursement berhasil di-approve oleh HR"
            });
        }

        [HttpPut("declinehr/{id}")]
        public IActionResult DeclineReimbursementByHR(string id, ChangeStatusReimbursement changeReimbursement)
        {
            bool isApprove = false;
            var result = _reimbursementRepositories.UpdateReimbursementStatusByHR(id, changeReimbursement, isApprove);

            if (result == ReimbursementRepositories.NOTFOUND)
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
                Message = "Reimbursement berhasil ditolak oleh HR"
            });
        }

        [HttpPut("approvefinance/{id}")]
        public IActionResult ApproveReimbursementByFinance(string id, ChangeStatusReimbursement approveReimbursement)
        {   bool isApprove = true;
            var result = _reimbursementRepositories.UpdateReimbursementStatusByFinance(id, approveReimbursement, isApprove);

            if (result == ReimbursementRepositories.NOTFOUND)
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
                Message = "Reimbursement berhasil di-approve oleh Finance"
            });
        }

        [HttpPut("declinefinance/{id}")]
        public IActionResult DeclineReimbursementByFinance(string id, ChangeStatusReimbursement declineReimbursement)
        {
            bool isApprove = false;
            var result = _reimbursementRepositories.UpdateReimbursementStatusByFinance(id, declineReimbursement, isApprove);

            if (result == ReimbursementRepositories.NOTFOUND)
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
                Message = "Reimbursement berhasil ditolak oleh Finance"
            });
        }
    }
}
