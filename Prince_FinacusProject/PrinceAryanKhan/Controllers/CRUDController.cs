using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrinceAryanKhan;
using PrinceAryanKhan.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Extensions.Configuration;

namespace PrinceAryanKhan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : ControllerBase

    {
       
        SqlConnection con = null;
       

        public CRUDController(IConfiguration config )
        {
            string str = config.GetConnectionString("Constr");
            con = new SqlConnection(str);
        }

        [HttpGet("PRODUCTS")]
        public ActionResult<IEnumerable<Products>> Retrieve()
        {

            SqlDataAdapter da = new SqlDataAdapter("Select * from Prince_Product", con);
            DataSet ds = new DataSet();
            con.Open();
            da.Fill(ds);

            List<Products> lst = new List<Products>();
            

            if (ds == null)
            {
                return NotFound("Table not found");
            }
            for (int n = 0; n < ds.Tables[0].Rows.Count; n++)
            {
                Products p = new Products();
                p.ProductId = (int)ds.Tables[0].Rows[n]["ProductId"];
                p.Name = (string)ds.Tables[0].Rows[n]["Name"];
                p.Description = (string)ds.Tables[0].Rows[n]["Description"];
                p.Price = (double)ds.Tables[0].Rows[n]["Price"];
                p.Category = (string)ds.Tables[0].Rows[n]["Category"];

                lst.Add(p);

            }

            return lst;

        }
        [HttpGet("PRODUCTS/{id}")]
        public ActionResult<IEnumerable<Products>> Retrieve_One(int id)

        {
            con.Open();
            SqlCommand cmd = new SqlCommand($"Select * from Prince_Product where ProductId={id}", con);
            SqlDataReader dr = null;

            dr = cmd.ExecuteReader();

            List<Products> lst = new List<Products>();
            Products p = new Products();
            if (!dr.HasRows)
            {
                return NotFound("This record doesn't exist with ProductId = " + id);
            }
            while (dr.Read())
            {
                p.ProductId = dr.GetInt32(0);
                p.Name = dr.GetString(1);
                p.Description = dr.GetString(2);
                p.Price = dr.GetDouble(3);
                p.Category = dr.GetString(4);
                lst.Add(p);
            }
            return lst;

        }
        [HttpPost("PRODUCTS")]
        public ActionResult<string> Create(Products p)
        {
          
            con.Open();
            SqlCommand cmd = new SqlCommand("Insert into Prince_Product values(@p1,@p2,@p3,@p4,@p5)", con);
            cmd.Parameters.Add(new SqlParameter("p1", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("p2", SqlDbType.VarChar, 250));
            cmd.Parameters.Add(new SqlParameter("p3", SqlDbType.VarChar, 250));
            cmd.Parameters.Add(new SqlParameter("p4", SqlDbType.Float));
            cmd.Parameters.Add(new SqlParameter("p5", SqlDbType.VarChar, 15));
            cmd.Parameters["p1"].Value = p.ProductId;
            cmd.Parameters["p2"].Value = p.Name;
            cmd.Parameters["p3"].Value = p.Description;
            cmd.Parameters["p4"].Value = p.Price;
            cmd.Parameters["p5"].Value = p.Category;
            int a = cmd.ExecuteNonQuery();
            if (a == 0)
            {
                return BadRequest("Insert failed please enter correct values in all parameters");
            }
            return a + " Record inserted";
        }
        [HttpDelete("PRODUCTS/{id}")]
        public ActionResult<string> Delete(int id)
        {
            SqlCommand cmd = new SqlCommand($"Delete from Prince_product where ProductId={id}", con);
            con.Open();
            int a = cmd.ExecuteNonQuery();
            if(a==0)
            {
                return NotFound("Record not found to delete with Product_Id " + id);
            }
            return a + "Record Deleted";

        }
        [HttpPut("PRODUCTS/{id}")]
        public ActionResult<string> Update(Products p)
        {
            
            SqlCommand cmd = new SqlCommand("Update Prince_Product set Name=@p1 where ProductId=@p2 ", con);
            cmd.Parameters.Add(new SqlParameter("p1", SqlDbType.VarChar, 250));
            cmd.Parameters.Add(new SqlParameter("p2", SqlDbType.Int));
            cmd.Parameters["p1"].Value = p.Name;
            cmd.Parameters["p2"].Value = p.ProductId;
            con.Open();
            int a = cmd.ExecuteNonQuery();
            if(a==0)
            {
                return NotFound("Record not found to update with Product_Id " + p.ProductId);
            }
            return a + "Record Updated";
        }



    }
}
