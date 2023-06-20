using INS_Group_Coding_Challenge.Data;
using INS_Group_Coding_Challenge.Models;
using INS_Group_Coding_Challenge.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INS_Group_Coding_Challenge.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class UserController: ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        //show all users
        [HttpGet("GetAllUsers")]
        public ActionResult<IEnumerable<UserDTO>> GetUsers()
        {
            return Ok(_db.Users.ToList());
        }


        //find user by id
        [HttpGet("GetUserById", Name ="GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDTO> GetUser(int id)
        {
            var user = _db.Users.FirstOrDefault(e => e.Id == id);
            if (id == 0) {
                return BadRequest();
            }
            if (user == null) { 
                return NotFound();
            }
            return Ok(user);
        }

        //first option to update username
        [HttpPost("UpdateAccountName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUser([FromBody] UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest();
            }
            var user = _db.Users.FirstOrDefault(e => e.Id == userDTO.id);
            if (user != null) { 
            user.UserName = userDTO.UserName;
            }
            _db.SaveChanges();
            return Ok(user);
        }

        //change user name
        //a batter way to update username , 2nd option
        //[HttpPatch("UpdateUserName/{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public IActionResult UpdateUserName(int id, JsonPatchDocument<UserDTO> patchDTO) {
        //    if (patchDTO == null || id == 0) {
        //        return BadRequest();
        //    }
        //    var user = _db.Users.AsNoTracking().FirstOrDefault(e => e.Id == id);
        //    UserDTO userDTO = new()
        //    {
        //        Id= user.Id,
        //        UserName = user.UserName
        //    };
        //    if (user == null) {
        //        return BadRequest();
        //    }
        //    patchDTO.ApplyTo(userDTO, ModelState); //if there is any errors, store error in modelState
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    User u = new User() {
        //        Id = userDTO.Id,
        //        UserName = userDTO.UserName,
        //    };
        //    _db.Users.Update(u);
        //    _db.SaveChanges();
        //    return Ok(u);
        //}

        //create a user
        [HttpPost("CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CreateUser([FromBody] CreateUserReqeuest userRequest) {
            if (userRequest == null) {
                return BadRequest(userRequest);
            }
          
            User user = new()
            {
                UserName = userRequest.UserName,
            };
            _db.Users.Add(user);
            _db.SaveChanges();
            return CreatedAtRoute("GetUserById", new { id= user.Id}, userRequest);
        }

        //add a note
        [HttpPost("AddNote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddNote([FromBody] AddNoteRequest noteRequest) {
            var user = _db.Users.Where(e => e.Id == noteRequest.UserId).Include(e => e.Notes).FirstOrDefault();
            if (user != null) {
               // user.Notes = new List<Note>();
                user.Notes.Add(
                    new Note()
                    {
                        Content = noteRequest.Content
                    }
                );
            }
            _db.SaveChanges();
            return CreatedAtRoute("GetUserById", new { id = user.Id }, noteRequest); ;
        }

        //list all notes under a certain user
        [HttpGet("GetNotesByUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetNotesByUser(int userId)
        {
            if (userId == 0)
            {
                return BadRequest();
            }
           
            var user = _db.Users.Where(e => e.Id == userId).Include(e => e.Notes).FirstOrDefault();
           
            return user==null ? NotFound() : Ok (user.Notes);
        }

        //login , no auth
        [HttpGet("Login")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult login(LoginRequest myrequest)
        {
            if (myrequest.accountName == null)
            {
                return BadRequest();
            }

            var user = _db.Users.Where(e => e.UserName.ToLower() == myrequest.accountName.ToLower()).Include(e => e.Notes).FirstOrDefault();

            return user == null ? NotFound() : Ok(user);
        }
    }
}
