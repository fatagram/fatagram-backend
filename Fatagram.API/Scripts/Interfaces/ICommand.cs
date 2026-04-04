using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Scripts
{
    public interface ICommand
    {
        string Name { get; }
        Task Execute(string[] args);
    }
}
