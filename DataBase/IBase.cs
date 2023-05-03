using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface IBase
    {
        void Salvar();
        void Excluir();
        int Key();
        void CriarTabela();
        List<IBase> Todos();
        List<IBase> Buscar();
    }
}
