using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    //[Authorize(Roles = "HR")]
    public class TitlesController : ControllerBase
    {
        private TitleRepositories _titleRepositories;

        public TitlesController(TitleRepositories titleRepositories) 
        { 
            _titleRepositories = titleRepositories; 
        }

        [Authorize(Roles = "HR, Employee, Finance")]
        [HttpGet]
        public IActionResult GetAllTitles()
        {
            var titles = _titleRepositories.GetAllTitles();

            if (titles.Count() == 0)
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
                Message = $"{titles.Count()} data ditemukan",
                Data = titles
            });
        }

        [HttpGet("{titleId}")]
        public IActionResult GetTitleById(string titleId)
        {
            var title = _titleRepositories.GetTitleById(titleId);
            if (title == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Data {titleId} tidak ditemukan",
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = $"Data {title} berhasil ditemukan",
                Data = title
            });
        }

        [HttpPost]
        public IActionResult AddTitle(Title title)
        {
            if (String.IsNullOrWhiteSpace(title.Title_Name))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Nama title harus diisi",
                });
            }
            else if (title.Reimburse_Limit == null || title.Reimburse_Limit == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Reimburse limit harus diisi",
                });
            }

            var addTitle = _titleRepositories.AddTitles(title);

            if (addTitle <= 0)
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
                Data = title
            });
        }

        [HttpPut("{titleId}")]
        public IActionResult UpdateTitle(string titleId, Title title)
        {
            if (String.IsNullOrWhiteSpace(title.Title_Name))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Nama title harus diisi",
                });
            }
            else if (String.IsNullOrWhiteSpace(title.Id_Title))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"ID title harus diisi",
                });
            }
            else if (title.Reimburse_Limit == null || title.Reimburse_Limit == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"Reimburse limit harus diisi",
                });
            }

            if (titleId != title.Id_Title)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"ID tidak sama",
                });
            }

            var updatedTitle = _titleRepositories.UpdateTitle(title);

            if (updatedTitle == TitleRepositories.NOTFOUND)
            {
                return NotFound(new
                {
                    StatusCode = 400,
                    Message = $"ID {titleId} tidak ditemukan",
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = $"Data {titleId} berhasil diupdate",
                Data = title
            });
        }

        [HttpDelete("{titleId}")]
        public IActionResult DeleteTitle(string titleId)
        {
            try
            {
                int delete = _titleRepositories.DeleteTitleById(titleId);

                if (delete == TitleRepositories.NOTFOUND)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = $"Data {titleId} tidak ditemukan",
                    });
                }
                else if (delete == TitleRepositories.INUSE)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = $"Data {titleId} tidak bisa dihapus karena sedang digunakan oleh akun lain",
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Data {titleId} berhasil dihapus",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Terjadi kesalahan saat menghapus data",
                });
            }
        }

    }
}
