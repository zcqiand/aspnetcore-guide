namespace AutoMapper.WebApi01
{
    public class DomainToDTOProfile : Profile
    {
        public DomainToDTOProfile()
        {
            CreateMap<Todo, TodoDTO>();
        }
    }
}
