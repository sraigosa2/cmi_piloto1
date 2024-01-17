﻿using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Ws_OLS.Clases
{
    public class Facturas
    {
        //METODOS DE LLAMADA

        //private Database Db;

        string connectionString = ConfigurationManager.ConnectionStrings["IPES"].ConnectionString;
        //string connectionStringH = ConfigurationManager.ConnectionStrings["IPESH"].ConnectionString;

        //public Facturas()
        //{
        //	Db = DatabaseFactory.CreateDatabase();
        //}


        //OBTIENE CANTIDAD DE FILAS POR RUTA Y FECHA DONDE SEA FACTURA
        public DataTable CantidadFacturas(int ruta, string FC_tipo, string FC_estado, string fecha, long facNum, int doc)
        {
            DataTable dt = new DataTable();

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery;

                if (doc == 7) //7 ANULACIONES
                {
                    if (facNum == -1)// ANULACIONES MASIVAS
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
									  AND FeLAnulacionNumero IS NULL";
                    }
                    else          //ANULACIONES INDIVIDUALES
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
									  AND Numero=@numero";
                    }
                }
                else
                {
                    if (facNum == -1)
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
                                      AND FELAutorizacion IS NULL";
                    }
                    else
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
									  AND Numero=@numero
                                      AND ((len(FELAutorizacion)<10 OR  FELAutorizacion IS NULL)
                                      OR (len(FELSerie)<10 OR  FELSerie IS NULL)
                                      OR (len(FELNumero)<10 OR  FELNumero IS NULL))";
                    }
                }


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@FC", FC_tipo);
                    cmd.Parameters.AddWithValue("@estado", FC_estado);
                    if (facNum != -1)
                    {
                        cmd.Parameters.AddWithValue("@numero", facNum);
                    }
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(dt);
                    //dsSumario.Tables.Add(dt);


                }

                cnn.Close();
            }

            return dt;
        }

        //OBTIENE CANTIDAD DE FILAS POR RUTA Y FECHA DONDE SEA FACTURA
        public DataTable CantidadFacturasPreImpresas(int ruta, string fecha, long facNum, int doc)
        {
            DataTable dt = new DataTable();

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery;

                if (doc == 7) //7 ANULACIONES
                {
                    if (facNum == -1)// ANULACIONES MASIVAS
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
									  AND FeLAnulacionNumero IS NULL";
                    }
                    else          //ANULACIONES INDIVIDUALES
                    {
                        sqlQuery = @"SELECT *
									  FROM HandHeld.FacturaEBajada
									  WHERE idRuta=@ruta 
									  AND CAST(Fecha AS DATE)=@fecha 
                                      AND TipoDocumento=@FC
                                      AND estado=@estado
									  AND Numero=@numero";
                    }
                }
                else
                {
                    if (facNum == -1)
                    {
                        sqlQuery = @"SELECT *
									  FROM Facturacion.FacturaE 
									  WHERE idRutaReparto=@ruta 
									  AND CAST(FechaFactura AS DATE)=@fecha 
                                      AND Estado=0
                                      AND FELAutorizacion IS NULL";
                    }
                    else
                    {
                        sqlQuery = @"SELECT *
									  FROM Facturacion.FacturaE 
									  WHERE idRutaReparto=@ruta 
									  AND CAST(FechaFactura AS DATE)=@fecha 
                                      AND Estado=0
									  AND idFactura=@numero
                                      AND FELAutorizacion IS NULL";
                    }
                }


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    //cmd.Parameters.AddWithValue("@estado", FC_estado);
                    if (facNum != -1)
                    {
                        cmd.Parameters.AddWithValue("@numero", facNum);
                    }
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(dt);
                    //dsSumario.Tables.Add(dt);


                }

                cnn.Close();
            }

            return dt;
        }

        //OBTIENE RESOLUCION 
        public string GetResolucion(int ruta, string idSerie)
        {
            string resolucion = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT resolucion 
                                    FROM Facturacion.Series 
                                    WHERE idSerie=@idSerie AND idRuta=@ruta AND estado ='ACT'";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        resolucion = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return resolucion;
        }

        //OBTIENE TOKEN DE FECHA ACTUAL
        public string GetTokenNow(string fecha)
        {
            string resolucion = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT ISNULL((SELECT TOP 1 Token 
                                    FROM Facturacion.TokenFEL 
                                    WHERE CAST(Fecha AS DATE) =@Fecha), 0) Token";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        resolucion = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return resolucion;
        }


        //INSERTAR TOKEN EN LA BASE DE DATOS
        public void InsertaToken(string TokenI)
        {

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT idSerie
                //                                FROM Facturacion.Series 
                //                                WHERE idRuta=@ruta AND CAST(FechaIngreso AS DATE)=@fecha AND idTipoSerie=@idTipo ";
                string sqlQuery = @"INSERT INTO Facturacion.TokenFEL (Fecha,Token) VALUES
	                                (GETDATE(),@Token)";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@Token", TokenI);
                    cmd.ExecuteNonQuery();
                    //dsSumario.Tables.Add(dt);
                }

                cnn.Close();
            }
        }



        //OBTIENE RESTINICIO
        public string GetResInicio(int ruta, string idSerie)
        {
            string resInicio = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT numeroDel 
                                    FROM Facturacion.Series 
                                    WHERE idRuta=@ruta AND idSerie=@idSerie";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        resInicio = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return resInicio;
        }

        //OBTIENE RESTFIN
        public string GetResFin(int ruta, string idSerie)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT numeroAl 
                                    FROM Facturacion.Series 
                                    WHERE idRuta=@ruta AND idSerie=@idSerie";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE RESTFIN
        public string GetRestFecha(int ruta, string idSerie)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT FechaAutorizacion 
                                    FROM Facturacion.Series 
                                    WHERE idRuta=@ruta AND idSerie=@idSerie";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NIT
        public string GetNit(int ruta, string idSerie)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT nit 
                                    FROM Facturacion.Series 
                                    WHERE idRuta=@ruta AND idSerie=@idSerie";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE ORDEN DE COMPRA
        public string GetOrdenCompraHH(string pedido)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT no_orden 
                                    FROM dbo.Pedidos
                                    WHERE IdPedido=@pedido";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@pedido", pedido);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE ORDEN DE COMPRA
        public string GetOrdenCompraPreImpresa(string pedido)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"select p.no_orden from facturacion.facturae fe
                                    inner join sap.pedidos sp on sp.NumPedidoSAP = fe.idPedido
                                    inner join dbo.pedidos p on sp.PedidoOriginal = p.IdPedido
                                    where idfactura =@pedido";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@pedido", pedido);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE FECHA EMISION
        public string GetFechaEmision(int ruta, int idTipo, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT FechaHora 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE GETIDEMPLEADO
        public string GetIdEmpleado(int ruta, int idTipo, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT idempleado 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE SERIE
        public int GetIdSerie(int ruta, string tipo, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT idSerie 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero and TipoDocumento=@Tipo";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToInt32(data);
        }

        //OBTIENE NUMERO SERIES
        public string GetNumSerie(int ruta, string idSerie)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT numeroSerie 
                                    FROM Facturacion.Series
                                    WHERE idRuta=@ruta AND idSerie=@idSerie";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@idSerie", idSerie);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NOMBRE DE ESTABLECIMIENTO
        public string GetNombreEstablecimiento(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT NombreNegocio
									FROM SAP.Clientes
									WHERE IdCliente=@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NOMBRE DEL USUARIO
        public string GetNombreUsuario(string id)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT nombreEmpleado
									FROM EmpleadosHH
									WHERE IdEmpleado=@id";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE TIPO DE DOCUMENTOS
        public string GetTipoDocumento(int ruta, int idTipo, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT TipoDocumento 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NUIT CLIENTE
        public string GetNITCliente(string cliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.DocumentoCliente 
									FROM Clientes.Documentos CD 
									INNER JOIN Clientes C ON C.IdClientePropietario = CD.IdCliente 
									WHERE CD.IdTiposDocumento =1 AND C.IdCliente =@IdCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE DUI CLIENTE
        public string GetDUI(string cliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.DocumentoCliente 
									FROM Clientes.Documentos CD 
									INNER JOIN Clientes C ON C.IdClientePropietario = CD.IdCliente 
									WHERE CD.IdTiposDocumento =2 AND C.IdCliente =@IdCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NRC CLIENTE
        public string GetNRC(string cliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.DocumentoCliente 
									FROM Clientes.Documentos CD 
									INNER JOIN Clientes C ON C.IdClientePropietario = CD.IdCliente 
									WHERE CD.IdTiposDocumento =3 AND C.IdCliente =@IdCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE CODIGO CLIENTE
        public string GetCodigoCliente(int ruta, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT IdCliente 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE CODIGO CLIENTE
        public string GetNombreCliente(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT NombreCompleto 
                                    FROM dbo.Clientes
                                    WHERE IdCliente=@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE DIRECCION 
        public string GetDireccion(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Direccion 
                                    FROM DireccionesClientes
                                    WHERE IdCliente=@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE DEPARTAMENTO 
        public string GetDepartamento(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.NombreCompleto 
									FROM DireccionesClientes DC 
									INNER JOIN Catalogos.Departamentos CD ON CD.IdDepartamento = DC.IdDepartamento 
									WHERE DC.IdCliente =@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetIdDepartamento(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.CodigoDepartamento 
									FROM DireccionesClientes DC 
									INNER JOIN Catalogos.Departamentos CD ON CD.IdDepartamento = DC.IdDepartamento 
									WHERE DC.IdCliente =@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE DEPARTAMENTO 
        public string GetMunicipio(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CD.NombreCompleto 
									FROM DireccionesClientes DC 
									INNER JOIN Catalogos.Municipios CD ON CD.IdMunicipio = DC.IdMunicipio 
									WHERE DC.IdCliente =@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetIdMunicipio(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT RIGHT(CD.CodigoMunicipio, 2) as CodigoMunicipio
									FROM DireccionesClientes DC 
									INNER JOIN Catalogos.Municipios CD ON CD.IdDepartamento = DC.IdDepartamento 
									WHERE DC.IdCliente =@idCliente AND CD.IdMunicipio =DC.IdMunicipio ";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE GIRO NEGOCIO 
        public string GetGiroNegocio(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT giro_negocio
									FROM SAP.Clientes
									WHERE IdCliente=@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetGiroNegocio2(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //       string sqlQuery = @"SELECT ActividadEconomica
                //FROM SAP.PersonaT 
                //                           WHERE CodFuncion = 15 AND RIGHT('0000000000' + LTRIM(RTRIM(STR(CodCliente))), 10) LIKE '%' + @idCliente + '%'";

                string sqlQuery = @"SELECT ActividadEconomica
                                    FROM SAP.PersonaT PT
                                    INNER JOIN Clientes C ON C.IdClientePropietario = PT.CodCliente
                                    WHERE CodFuncion = 15 AND IdCliente = @idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    //cmd.Parameters.AddWithValue("@idCliente", idCliente.Trim().ToString());
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetActividadEconomica(string actividad)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Descripcion
									FROM Catalogos.ActividadEconomica  
                                    WHERE CodActividadEconomica = @actividad";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@actividad", actividad);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE CONDICION PAGO
        public string GetCondicionPago(int ruta, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT IdCondicionPago
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE CONDICION PAGO
        public double GetVentaTotal(int ruta, string fecha, string numero)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Total
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }


        //OBTIENE MONTO EN LETRAS
        public string GetMontoLetras(double monto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT dbo.Numeros_a_Letras(@monto)";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@monto", monto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        public double GetCantidadTotal(int ruta, string numero)
        {
            string data = "";

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT SUM(Unidades)
                                    FROM HandHeld.FacturaDBajada 
                                    WHERE idRuta=@ruta AND Numero=@numero ";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }


        public double GetCantidadTotalPreImpresa(string numero)
        {
            string data = "";

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT SUM(Unidades)
                                    FROM Facturacion.FacturaD 
                                    WHERE idFactura=@numero ";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }


        //OBTIENE CODIGO DE CLIENTE PRINCIPAL
        public string GetCodigoClientePrincipal(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CodigoClientePrincipal
									FROM SAP.Clientes
									WHERE IdCliente=@idCliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE CENTRO
        public string GetCentro(string idRuta)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Centro
									FROM dbo.Rutas
									WHERE IdRuta=@idRuta";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idRuta", idRuta);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE CENTRO
        public string GetZonaRuta(string idRuta)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT DescripcionZona
									FROM Catalogos.Zonas Z
									INNER JOIN dbo.Rutas R ON R.IdZonaRuta=Z.IdZonaRuta
									WHERE IdRuta=@idRuta";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idRuta", idRuta);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE RUTA VENTA
        public string GetRutaVenta(string idCliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT idruta
                //					FROM dbo.RutasClientes
                //					WHERE idcliente=@idCliente";
                string sqlQuery = @"SELECT Ruta RutaVenta 
								   FROM dbo.rutasclientes rc
                                   INNER JOIN dbo.rutas r ON rc.idruta = r.idruta
							       WHERE idcliente = @idCliente AND rc.idtiporuta = 1";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE RUTA REPARTO
        public string GetRutaReparto(string idRuta)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CodigoRuta
									FROM dbo.Rutas
									WHERE IdRuta=@idRuta AND IdTipoRuta=2";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idRuta", idRuta);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE TELEFONO
        public string GetTelefono(string cliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT RTRIM(TelMovil)
                                    FROM SAP.Clientes
                                    WHERE IdCliente =@Cliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@cliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE CORREO
        public string GetCorreo(string cliente)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT RTRIM(CorreoElectronico)
                                    FROM SAP.Clientes
                                    WHERE IdCliente =@Cliente";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@cliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE RUTA REPARTO
        public string GetCodigoRutaVenta(string idRuta)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT CodigoRuta
									FROM dbo.Rutas
									WHERE IdRuta=@idRuta AND IdTipoRuta=1";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idRuta", idRuta);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE SECUENCIA
        public string GetSecuencia(string num)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Secuencia
									FROM dbo.Pedidos
									WHERE IdPedido=@num1";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@num1", num);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        /***************CAMPOS NUEVOS FEL****************/

        //CAMPO FEL-OBTIENE SI ES RUTA FEL O NO
        public bool GetRutaFEL(int ruta)
        {
            bool felData = false;
            string campo = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT FacturaFelOLS 
									FROM Pedidos.Rutas 
									WHERE IdRuta=@ruta AND IdTipoRuta =2";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        campo = dr[0].ToString();
                    }

                    if (campo == "1")
                    {
                        felData = true;
                    }
                }

                cnn.Close();
            }

            return felData;
        }

        //CAMPO FEL-OBTIENE CODIGO DE GENERACION
        public string GetCodigoGeneracion(int ruta, string idTipo, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELSerie,''), '0') FELSerie 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero AND TipoDocumento=@idTipo";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE CODIGO DE NUM CONTROL
        public string GetCodigoNumControl(int ruta, string idTipo, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELNumero,''), '0') FELNumero 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero AND TipoDocumento=@idTipo";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE CODIGO DE SELLO
        public string GetCodigoSello(int ruta, string idTipo, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELAutorizacion,''), '0') FELAutorizacion 
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero AND TipoDocumento=@idTipo";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetTipoDocPreImpresa(int ruta, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT TipoFactura
                                    FROM Facturacion.FacturaE
                                    WHERE idRutaReparto=@ruta AND CAST(FechaFactura AS DATE)=@fecha and idFactura=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE CODIGO DE GENERACION
        public string GetTipoDoc(int ruta, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT TipoDocumento
                                    FROM HandHeld.FacturaEBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE CODIGO DE GENERACION
        public string GetCodigoGeneracionPreImpresa(int ruta, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELSerie,''), '0') FELSerie 
                                    FROM Facturacion.FacturaE
                                    WHERE idRutaReparto=@ruta AND CAST(FechaFactura AS DATE)=@fecha and idFactura=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE NUMERO DE CONTROL
        public string GetNumControlPreImpresa(int ruta, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELSerie,''), '0') FELNumero 
                                    FROM Facturacion.FacturaE
                                    WHERE idRutaReparto=@ruta AND CAST(FechaFactura AS DATE)=@fecha and idFactura=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //CAMPO FEL-OBTIENE CODIGO DE SELLO
        public string GetCodigoSelloPreImpres(int ruta, string fecha, string numero)
        {
            string data = "0";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT COALESCE(NULLIF(FELAutorizacion,''), '0') FELAutorizacion 
                                    FROM Facturacion.FacturaE
                                    WHERE idRutaReparto=@ruta AND CAST(FechaFactura AS DATE)=@fecha and idFactura=@numero";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        /**********************DETALLE***********************/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="numero"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>

        //OBTIENE DETALLE POR FACTURAS
        public DataTable CantidadDetalle(int ruta, string numero, string fecha, int idSerie)
        {
            DataTable dt = new DataTable();

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT IdProductos
                string sqlQuery = @"SELECT *
                                    FROM HandHeld.FacturaDBajada 
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha AND Numero=@numero AND IdSerie=@serie
									ORDER BY IdProductos ASC";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@serie", idSerie);
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(dt);
                    //dsSumario.Tables.Add(dt);
                }

                cnn.Close();
            }

            return dt;
        }

        //OBTIENE DETALLE POR FACTURAS
        public DataTable CantidadDetallePreImpresa(string numero)
        {
            DataTable dt = new DataTable();

            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT *
                                    FROM Facturacion.FacturaD 
                                    WHERE idFactura=@numero
									ORDER BY IdProductos ASC";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(dt);
                    //dsSumario.Tables.Add(dt);
                }

                cnn.Close();
            }

            return dt;
        }

        //OBTIENE CANTIDAD DE UNIDADES POR CADA DETALLE
        public double GetUnidadesDetallePreImpresa(string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Unidades 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        public double GetPesoDetallePreImpresa(string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Peso 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE CANTIDAD DE UNIDADES POR CADA DETALLE
        public double GetCantidadDetalle(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Unidades 
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE CANTIDAD DE UNIDADES POR CADA DETALLE
        public double GetCantidadDetallePreImpresa(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Unidades 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto and NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }



        //OBTIENE NOMBRE DEL PRODUCTO
        public string GetNombreProducto(string producto)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT NombreCompleto 
                                    FROM SAP.Productos 
                                    WHERE idProducto = @producto";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NOMBRE DEL PRODUCTO
        public string GetNumeroFacturaSGR_Preimpresa(string pedido)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"select sp.PedidoOriginal 
                                    from facturacion.facturae fe
                                    inner join sap.pedidos sp on fe.idpedido = sp.NumPedidoSAP
                                    where fe.idpedido=@pedido";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@pedido", pedido);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE NOMBRE DEL PRODUCTO
        public string GetUsuarioGeneraPreImpresa(string idUser)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @" SELECT TOP 1 U.NombreCompleto 
                                     FROM ADMAPP.Config.Usuarios U
                                     INNER JOIN Facturacion.FacturaE F ON F.IdUsuarioGenera = U.Id 
                                     WHERE F.IdUsuarioGenera  =@idUser";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idUser", idUser);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE PESO DEL PRODUCTO 
        public double GetPesoProductoDetalle(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Peso 
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }


        //OBTIENE PESO DEL PRODUCTO 
        public double GetPesoProductoDetallePreImpresa(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Peso 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto AND NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE PRECIO UNITARIO DEL DETALLE
        public decimal GetPrecioUnitarioDetalle(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT (PrecioUnitario + DescuentoPorPrecio)
                string sqlQuery = @"SELECT FORMAT((PrecioUnitario + DescuentoPorPrecio),'N4', 'es-GT')
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDecimal(data);
        }

        //OBTIENE PRECIO UNITARIO DEL DETALLE
        public decimal GetPrecioUnitarioDetallePreImpresaCCF(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT (PrecioUnitario + DescuentoPorPrecio)
                //string sqlQuery = @"SELECT FORMAT((PrecioSinImpuesto + Descuento),'N4', 'es-GT')
                string sqlQuery = @"SELECT FORMAT((D.PrecioSinImpuesto/H.Multiplo),'N4', 'es-GT')
                                    FROM Facturacion.FacturaD D
                                    INNER JOIN HandHeld.PreciosProducto H ON H.IdProductos=D.idProductos
                                    WHERE D.idFactura=@numero AND D.idProductos=@producto AND NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDecimal(data);
        }



        //OBTIENE PRECIO UNITARIO DEL DETALLE
        public decimal GetPrecioUnitarioDetalleFAC(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {


                cnn.Open();
                //string sqlQuery = @"SELECT ROUND(((PrecioUnitario * 0.13) + preciounitario + DescuentoPorPrecio),2)
                //                                FROM HandHeld.FacturaDBajada
                //                                WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";
                string sqlQuery = @"SELECT FORMAT((CASE WHEN P.UnidadFacturacion= 1 THEN D.Valor/ D.Unidades ELSE D.Valor / D.Peso END),'N4', 'es-GT')
                                  FROM HandHeld.FacturaDBajada D
                                  INNER JOIN Reparto.VistaProducto P ON P.IdProductos = D.IdProductos
                                  WHERE D.idRuta=@ruta AND D.Numero=@numero AND D.IdProductos=@producto";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
                //Double dd = Convert.ToDouble(data.Replace(',', '.'), CultureInfo.InvariantCulture);
            }

            return Convert.ToDecimal(data);
        }

        //COMPRUEBA UNIDAD DE MEDIDA
        public string CompruebaUnidadMedida(string producto)
        {
            string data = "1";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {


                cnn.Open();
                //string sqlQuery = @"SELECT ROUND(((PrecioUnitario * 0.13) + preciounitario + DescuentoPorPrecio),2)
                //                                FROM HandHeld.FacturaDBajada
                //                                WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";
                string sqlQuery = @"SELECT UnidadFacturacion
                                  FROM Reparto.VistaProducto
                                  WHERE IdProductos=@producto";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
                //Double dd = Convert.ToDouble(data.Replace(',', '.'), CultureInfo.InvariantCulture);
            }

            return data;
        }

        public string CompruebaUnidadMedidaPreimpresa(string producto)
        {
            string data = "1";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {


                cnn.Open();
                //string sqlQuery = @"SELECT ROUND(((PrecioUnitario * 0.13) + preciounitario + DescuentoPorPrecio),2)
                //                                FROM HandHeld.FacturaDBajada
                //                                WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";
                string sqlQuery = @"select pp.IdProductos, pp.UnidadMedida, vp.UnidadFacturacion
                                    into #productosEspeciales
                                    from handheld.preciosproducto pp
                                    inner join reparto.vistaproducto vp on pp.IdProductos = vp.IdProductos
                                    where pp.UnidadMedida = 'LB' and vp.UnidadFacturacion <> 3
                                    SELECT CASE WHEN PE.UnidadFacturacion=1 THEN  3 ELSE    VP.UnidadFacturacion END Unidadfacturacion
                                    FROM Reparto.VistaProducto vp left join #productosEspeciales pe on vp.IdProductos =pe.idproductos
                                    WHERE VP.IdProductos =@producto";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
                //Double dd = Convert.ToDouble(data.Replace(',', '.'), CultureInfo.InvariantCulture);
            }

            return data;
        }



        //COMPRUEBA UNIDAD DE MEDIDA
        public string ObtieneMotivoAnulacion(int idMotivo)
        {
            string data = "1";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {


                cnn.Open();
                //string sqlQuery = @"SELECT ROUND(((PrecioUnitario * 0.13) + preciounitario + DescuentoPorPrecio),2)
                //                                FROM HandHeld.FacturaDBajada
                //                                WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";
                string sqlQuery = @"SELECT Descripcion 
                                    FROM [HandHeld].[VistaMotivoAnulacionFactura]
                                    WHERE IdMotivo =@idMotivo";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@idMotivo", idMotivo);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
                //Double dd = Convert.ToDouble(data.Replace(',', '.'), CultureInfo.InvariantCulture);
            }

            return data;
        }

        //OBTIENE PRECIO UNITARIO DEL DETALLE
        public decimal GetPrecioUnitarioDetalleFACPreImpreso(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {


                cnn.Open();
                //string sqlQuery = @"SELECT ROUND(((PrecioUnitario * 0.13) + preciounitario + DescuentoPorPrecio),2)
                //                                FROM HandHeld.FacturaDBajada
                //                                WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";
                string sqlQuery = @"SELECT FORMAT((CASE WHEN P.UnidadFacturacion= 1 THEN (D.Total+D.ValeMerma)/D.Unidades ELSE (D.Total+D.ValeMerma)/ D.Peso END),'N4', 'es-GT')
                                  FROM Facturacion.FacturaD D
                                  INNER JOIN Reparto.VistaProducto P ON P.IdProductos = D.idProductos
                                  WHERE D.idFactura=@numero AND D.idProductos=@producto AND D.NumeroLinea=@linea";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                   
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
                //Double dd = Convert.ToDouble(data.Replace(',', '.'), CultureInfo.InvariantCulture);
            }

            return Convert.ToDecimal(data);
        }

        //OBTIENE PLU DEL PRODUCTO
        public string GetPLUProducto(string prod, string cliente)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT PLU 
                                    FROM Pedidos.ProductosCodigosPLU 
                                    WHERE IdProductos = @prod AND IdCliente=@cliente";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@prod", prod);
                    cmd.Parameters.AddWithValue("@cliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        public string GetPLUProductoPreimpresas(string prod, string cliente)
        {
            string data = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"select PLU from Facturacion.plu
                                   WHERE IdProductos = @prod AND IdCliente=(select codigoclienteprincipal from sap.clientes where idcliente=@cliente)
                                    union
                                    select
                                    plu
                                    From pedidos.ProductosCodigosPLU as t
                                    WHERE IdProductos = @prod AND IdCliente=(select codigoclienteprincipal from sap.clientes where idcliente=@cliente)";


                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@prod", prod);
                    cmd.Parameters.AddWithValue("@cliente", cliente);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //OBTIENE VENTAS GRAVADAS DETALLE
        public double GetVentasGravadasDetalle(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Valor 
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        public double GetVentasGravadasDetallePreImpresa(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT FORMAT(Total+ValeMerma, 'N2', 'es-GT')
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto AND NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE VENTAS GRAVADAS DETALLE CREDITO FISCAL
        public double GetVentasGravadasDetalleCCF(int ruta, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT (Valor-Iva) 
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE VENTAS GRAVADAS DETALLE CREDITO FISCAL
        public double GetVentasGravadasDetalleCCFPreImpresa(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT (Total-Iva) 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto and NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return Convert.ToDouble(data);
        }

        //OBTIENE DESCUENTO POR PRECIO
        public string GetDescuentoPrecioDetalle(int ruta, string fecha, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT DescuentoPorPrecio 
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE DESCUENTO POR PRECIO
        public string GetDescuentoPrecioDetallePreImpresa(string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT Descuento 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }

        //OBTIENE DESCUENTO POR PRECIO
        public string GetValeMermaPrecioDetallePreImpresa(string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
                //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT ValeMerma 
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }

                cnn.Close();
            }

            return data;
        }


        //CAMPO FEL-OBTIENE IVA DE LA LINEA
        public decimal GetIVALineaFac(int ruta, string fecha, string numero, string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT ISNULL(Iva, 0) Iva
                                    FROM HandHeld.FacturaDBajada
                                    WHERE idRuta=@ruta AND CAST(Fecha AS DATE)=@fecha and Numero=@numero AND IdProductos=@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ruta", ruta);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }



                cnn.Close();
            }

            return Convert.ToDecimal(data);
        }

        //CAMPO FEL-OBTIENE IVA DE LA LINEA
        public decimal GetIVALineaFacPreImpresa(string numero, string producto, string linea)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                //string sqlQuery = @"SELECT ISNULL(Iva, 0) Iva
                string sqlQuery = @"SELECT FORMAT((TOTAL+ValeMerma)-(TOTAL+ValeMerma)/1.13,'N2', 'es-GT')
                                    FROM Facturacion.FacturaD
                                    WHERE idFactura=@numero AND idProductos=@producto and NumeroLinea=@linea";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@linea", linea);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }



                cnn.Close();
            }

            return Convert.ToDecimal(data);
        }

        //CAMPO FEL-OBTIENE UNIDADA FACTURACION
        public int GetUnidadFacturacion(string producto)
        {
            string data = "";
            //using (SqlConnection cnn = new SqlConnection(connectionString)) using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlConnection cnn = new SqlConnection(connectionString))
            //using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string sqlQuery = @"SELECT UnidadFacturacion 
									FROM Reparto.VistaProducto 
									WHERE IdProductos =@producto";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@producto", producto);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        data = dr[0].ToString();
                    }
                }



                cnn.Close();
            }

            return Convert.ToInt32(data);
        }




        //public string resolucion(int rr, int idSe)
        //{
        //	StringBuilder st = new StringBuilder();

        //	st.AppendLine("SELECT 0 idRuta , 'Seleccione' Ruta, '' OficinaVenta UNION ALL  ");
        //	st.AppendLine("(SELECT CodigoCompatibilidad idRuta, CONVERT(VARCHAR(6),CodigoCompatibilidad), OficinaVenta FROM catalogos.canaldistribucion) ORDER BY idRuta  ");
        //	using (DbCommand Cm = Db.GetSqlStringCommand(st.ToString()))
        //	{
        //		Db.ExecuteNonQuery(Cm);
        //		return Db.GetParameterValue(Cm, "ruta");

        //	}


        //}
    }
}