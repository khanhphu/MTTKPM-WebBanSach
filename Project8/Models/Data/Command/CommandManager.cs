using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Data.Command
{
    public class CommandManager
    {
        private readonly Stack<ICommand> _commands = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commands.Push(command); // Lưu lệnh vào stack
        }

        public void Undo()
        {
            if (_commands.Any())
            {
                var command = _commands.Pop(); // Lấy lệnh cuối cùng
                command.Undo(); // Thực hiện hoàn tác
            }
        }
    }
}