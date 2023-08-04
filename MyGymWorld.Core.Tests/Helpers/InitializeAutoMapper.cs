namespace MyGymWorld.Core.Tests.Helpers
{
    using AutoMapper;
    using MyGymWorld.Core.Mapping;

    public static class InitializeAutoMapper
    {
        public static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MyGymWorldMappingProfile>();
            }));
        }
    }
}
