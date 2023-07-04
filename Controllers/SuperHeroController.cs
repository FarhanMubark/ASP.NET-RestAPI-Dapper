using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHero()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }
        

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("Select * from SuperHeroes where id=@Id", 
                new {Id=heroId});
            
            return Ok(hero);
        }

        [HttpPost]

        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into SuperHeroes (name, firstname, lastname, place) values(@Name, @FirstName, @LastName, @Place)",hero); // for update or delete or create
            return Ok(await SelectAllHeroes(connection)); // same result return Ok(hero)
        }
        
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update SuperHeroes set name=@Name, firstname = @FirstName, lastname = @LastName, place = @Place where id=@Id", hero); // for update or delete or create
            return Ok(await SelectAllHeroes(connection)); // same result return Ok(hero)
        }
        
        [HttpDelete("{heroId}")]
        public async Task<ActionResult<SuperHero>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from SuperHeroes where id=@Id", new {Id = heroId}); // for update or delete or create
            return Ok(await SelectAllHeroes(connection)); // same result return Ok(hero)
        }

         private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            var heroes = await connection.QueryAsync<SuperHero>("Select * from SuperHeroes");
            return heroes;
        }
    }
}
