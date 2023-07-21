using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Web.DatingApp.API.Web.DatingApp.Database;
using Web.DatingApp.API.Web.DatingApp.Dtos;
using Web.DatingApp.API.Web.DatingApp.Entities;
using Web.DatingApp.API.Web.DatingApp.Helpers;
using Web.DatingApp.API.Web.DatingApp.Interfaces.Repositories;

namespace Web.DatingApp.API.Web.DatingApp.Implenentations.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DatingAppDbContext datingAppDbContext;
        private readonly IMapper mapper;

        public UserRepository(DatingAppDbContext datingAppDbContext, IMapper mapper)
        {
            this.datingAppDbContext = datingAppDbContext;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await datingAppDbContext.tbl_User
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = datingAppDbContext.tbl_User.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUserName);
            query = query.Where(u=>u.Gender != userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1)).ToDateTime(TimeOnly.MinValue);
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge)).ToDateTime(TimeOnly.MinValue);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            var items = await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(mapper.ConfigurationProvider), 
                userParams.PageNumber, 
                userParams.PageSize); 
            return items;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await datingAppDbContext.tbl_User.Include(p=>p.Photos).SingleOrDefaultAsync(s=>s.Id == id);
        }

        public async Task<AppUser> GetUserByNameAsync(string username)
        {
            return await datingAppDbContext.tbl_User.Include(p => p.Photos).SingleOrDefaultAsync(s => s.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await datingAppDbContext.tbl_User.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllChanges()
        {
            return await datingAppDbContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser appUser)
        {
            datingAppDbContext.Entry(appUser).State = EntityState.Modified;
        }
    }
}
