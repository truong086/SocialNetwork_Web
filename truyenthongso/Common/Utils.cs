using truyenthongso.Models;

namespace truyenthongso.Common
{
    public class Utils
    {
        private readonly DBContext _context;
        public Utils(DBContext context)
        {
            _context = context;
        }

        public User? CheckUser (int id)
        {
            var checkAccount = _context.users.FirstOrDefault(x => x.id == id && !x.deleted);

            return checkAccount == null ? null : checkAccount;
        }
    }
}
