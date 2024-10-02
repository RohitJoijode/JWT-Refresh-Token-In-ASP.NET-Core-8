using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT_TOKEN_REFRESH_NET_CORE_8.DAL;
using JWT_TOKEN_REFRESH_NET_CORE_8.Contextes;

namespace JWT_TOKEN_REFRESH_NET_CORE_8
{
    public class DBEngine : DbContext
    {
        public DBEngine(DbContextOptions<DBEngine> options)
      :  base(options)
        {
        }

        public DbSet<Tbl_Users1> Tbl_Users1 { get; set; }
        public DbSet<Tbl_RefreshToken1> Tbl_RefreshToken1 { get; set; }

    }
}
