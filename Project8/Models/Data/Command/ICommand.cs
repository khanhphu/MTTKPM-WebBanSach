using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Models.Data.Command
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
