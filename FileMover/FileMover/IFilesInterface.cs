using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMover
{
    internal interface IFilesInterface
    {
        public void MoveFile(string fullFilePath, string fileName, int year);
    }
}
