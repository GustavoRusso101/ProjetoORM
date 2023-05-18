using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Base : IBase
    {
        private string connectionString = ConfigurationManager.AppSettings["MySql"];

        public List<IBase> Buscar()
        {
            var lista = new List<IBase>();
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                List<string> where = new List<string>();
                string chavePrimaria = string.Empty;
                foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    OpcoesBase opcoes = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if (opcoes != null)
                    {
                        if (opcoes.ChavePrimaria)
                        {
                            chavePrimaria = pi.Name + "=" + pi.GetValue(this);
                        }
                        else
                        {
                            if (TipoPropriedade(pi) == "varchar(255)" ||
                                TipoPropriedade(pi) == "datetime")
                                where.Add(pi.Name + "='" + pi.GetValue(this) + "'");
                            else
                                where.Add(pi.Name + "=" + pi.GetValue(this));

                        }
                    }
                }
                string sql;
                if (Key == 0)
                {
                    sql = "select * from " + this.GetType().Name + "s ";
                    if (where.Count > 0)
                    {
                        sql = " where " + string.Join(" or ", where.ToArray());
                    }
                }
                else
                {
                    sql="select * from "+ this.GetType().Name + "s where " + chavePrimaria;
                }
                MySqlCommand mysql = new MySqlCommand(sql, con);
                mysql.Connection.Open();
                MySqlDataReader mySqlDataReader = mysql.ExecuteReader();
                while(mySqlDataReader.Read())
                {
                    var obj = (IBase)Activator.CreateInstance(this.GetType());
                    foreach (PropertyInfo  info in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        OpcoesBase opcoes = (OpcoesBase)info.GetCustomAttribute
                            (typeof(OpcoesBase));
                            if (opcoes != null)
                        {
                            info.SetValue(obj, mySqlDataReader[info.Name]);
                        }
                        lista.Add(obj);
                    }
                }
                mysql.Connection.Close();
            }
            return lista;
        }
        //metodo que retorna propriedade
        private string TipoPropriedade(PropertyInfo pi)
        {
            switch(pi.PropertyType.Name)
            {
                case "Int32":
                    return "Int";
                case "Int64":
                    return "bigint";
                case "double":
                    return "secimal(9,2)";
                case "Datetime":
                    return "datetime";
                default:
                    return "varchar(255)";
            }
        }

        public void CriarTabela()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public int Key
        {
            get
            {
                foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | 
                    BindingFlags.Instance)) 
                {
                    OpcoesBase opcoes = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if (opcoes != null && opcoes.ChavePrimaria) 
                    {
                        return Convert.ToInt32(pi.GetValue(this));
                    }
                }
                return 0;
            }
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public List<IBase> Todos()
        {
            throw new NotImplementedException();
        }

        int IBase.Key()
        {
            throw new NotImplementedException();
        }
    }
}
