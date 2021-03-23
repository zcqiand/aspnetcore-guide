using Microsoft.AspNetCore.Mvc;
using System;

namespace AutoMapper.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly IMapper mapper; // <-添加此行
        public HomeController(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public TodoDTO Get()
        {
            var todo = new Todo
            {
                Id = new Guid("9a007349-23fa-45da-97a1-2999f923b5a7"),
                Name = "小明",
            };
            var todoDTO = mapper.Map<TodoDTO>(todo);
            return todoDTO;
        }
    }
}
