﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Model;
using Model.ViewModels;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountSystem.Controllers
{
    [Authorize]
    public class AccountClientController : Controller
    {
        private readonly IAccountClientService _repository;
        private readonly IClientService _repositoryClient;
        private readonly IRequestService _requestService;
        public AccountClientController(IAccountClientService repository
            , IClientService repositoryClient
            , IRequestService requestService)
        {
            _repository = repository;
            _repositoryClient = repositoryClient;
            _requestService = requestService;
        }



        [ValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult Add(int id)
        {
            var model = new AddAccountViewModel() { Id = id };
            if (_repositoryClient.Get(id) != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var model = _repository.Get(id);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        [HttpPost]
        public ActionResult Update(Account model)
        {

            var result = _repository.Update(model);
            if (result)
            {
                Alerts.Type = 8;
                return RedirectToAction("Index");
            }
            else
            {
                Alerts.Type = 7;
                ViewBag.response = Alerts.Type;
                return View(model);
            }
         
        }

        [HttpGet]
        public ActionResult Search(string parameter = "" , int page = 1)
        {
            var model = _repository.Search(parameter, page);
            if (model != null)
            {
                ViewBag.pagination = "1";
                ViewBag.parameter = parameter;
                return View("Index", model);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        [HttpGet]
        public ActionResult Detail(int id , int page = 1)
        {
            page = page == 0 ? 1 : page;
            var model = _repository.GetWithClientAndDebs(id , page);
            if (model != null)
            {
                ViewBag.response = Alerts.Type;
                Alerts.Type = 0;
                return View(model);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        [HttpPost]
        public ActionResult Pay (PayViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if(_repository.Get(model.AccountId)!= null){

                    if (_repository.Pay(model))
                    {
                        Alerts.Type = 14;
                    }
                    else
                    {
                        Alerts.Type = 15;
                    }

                    return RedirectToAction("Detail", new { id = model.AccountId });
                }
                else
                {
                    return HttpNotFound("404");
                }
            }
            else
            {
                Alerts.Type = 13;
                return RedirectToAction("Detail", new { id = model.AccountId });
            }
        }

        [HttpGet]
        public ActionResult PayOff(int id)
        {
            if (_repository.Get(id)!=null)
            {
                if (_repository.PayOff(id))
                {
                    Alerts.Type = 16;
                }

                return RedirectToAction("Detail", new { id });
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        public ActionResult GeneratePdf(int id)
        {
            string pathReport = Server.MapPath("~/Report/Report.rpt");
            string PathPdf = Server.MapPath("~/Report/Pdf/archivo.pdf");


            if (_repository.Get(id) != null)
            {
                if (_repository.GetWithClientAndDebs(id).TotalOfRegister != 0)
                {
                    if (_repository.Report(id, pathReport, PathPdf))
                    {
                        return File(PathPdf, "application/pdf", "archivo.pdf");
                    }
                    else
                    {
                        return RedirectToAction("PageNotFound", "Error");
                    }
                }
                else
                {
                    Alerts.Type = 17;
                    return RedirectToAction("Detail", new { id });
                }
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        //v2.0-------------------------------------------------
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(Account model)
        {
           model.CreatedAt = DateTime.Now;
           if (ModelState.IsValid)
           {
                if (!_repository.VerifyRequestAndClient(model.RequestId,model.ClientId))
                {
                    if (_repository.Add(model))
                    {
                        TempData["response"] = "Cuenta creada con exito";
                        TempData["icon"] = "success";
                    }
                    else
                    {
                        TempData["response"] = "Lo sentimos, no se ha creado la cuenta";
                        TempData["icon"] = "error";
                    }
                }
                else
                {
                    TempData["response"] = "Este cliente tiene una cuenta con esta solicitud, porfavor solicite una nueva cuenta";
                    TempData["icon"] = "error";
                }
            }
            else
            {
                TempData["response"] = "Lo sentimos, no se ha creado la cuenta";
                TempData["icon"] = "error";
            }
            return RedirectToAction("Index", "AccountClient");
        }

        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            var model = _repository.GetAllByPage(page);
            if (model!=null)
            {
                ViewBag.Clients = _repositoryClient.GetAll();
                ViewBag.Requests = _requestService.GetAll();
                if (TempData["response"] != null)
                {
                    ViewBag.Alert = TempData["response"].ToString();
                    ViewBag.Icon = TempData["Icon"].ToString();
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {

            var model = _repository.Get(id);
            if (model != null)
            {
                var vm = new AddAccountViewModel() { Id = id };
                ViewBag.PersonName = model.Name;
                return View(vm);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }
    }
}