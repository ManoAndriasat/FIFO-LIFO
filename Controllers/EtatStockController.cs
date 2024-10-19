using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Utils;
using Element;
using System;

public class EtatStockController : Controller
{
    private readonly NpgsqlConnection connection;

    public EtatStockController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult Index()
    {   
        if( HttpContext.Session.GetString("magasin") == null){
            HttpContext.Session.SetString("magasin", "1' or '1'='1");
        }

        string magasin=HttpContext.Session.GetString("magasin");
        ViewBag.ListMagasin = Magasin.AllMagasin(connection);
        ViewBag.EtatStock = EtatStock.EtatStockGlobal(magasin,connection);
        ViewBag.Vola= EtatStock.TotalVola( ViewBag.EtatStock );
        return View();
    }

    public IActionResult ChangeMagasin(string magasin){
        HttpContext.Session.SetString("magasin", magasin);
        return RedirectToAction("Index", "EtatStock");
    }

    public IActionResult ChangeDate(string firstDate,string lastDate){
        EtatStock.ChangeDate(connection, Convert.ToDateTime(firstDate) , Convert.ToDateTime(lastDate));
        return RedirectToAction("Index", "EtatStock");
    }

    public IActionResult DateNow(){
        EtatStock.ToDateNow(connection);
        return RedirectToAction("Index", "EtatStock");
    }
}
