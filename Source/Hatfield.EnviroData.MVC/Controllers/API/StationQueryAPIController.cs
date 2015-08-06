﻿using AutoMapper;
using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class StationQueryAPIController : ApiController
    {
        private readonly IWQDataRepository _wqDataRepository;
        private readonly IActionRepository _actionRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IWQVariableRepository _variableRepository;
        private readonly IResultRepository _resultRepository;

        public StationQueryAPIController(IActionRepository actionRepository, ISiteRepository siteRepository, IWQVariableRepository variableRepository, IResultRepository resultRepository, IWQDataRepository wqDataRepository)
        {
            _wqDataRepository = wqDataRepository;
            _actionRepository = actionRepository;
            _siteRepository = siteRepository;
            _variableRepository = variableRepository;
            _resultRepository = resultRepository;
        }

        [HttpGet]
        public IEnumerable<SiteViewModel> GetSites()
        {
            var sites = _siteRepository.GetAll();
            var items = Mapper.Map<IEnumerable<SiteViewModel>>(sites);
            return items;
        }

        [HttpGet]
        public SiteViewModel GetSingleSite(int locationId)
        {
            var sites = _siteRepository.GetAll().Where(x => x.SamplingFeatureID == locationId).FirstOrDefault();
            var item = Mapper.Map<SiteViewModel>(sites);
            return item;
        }

        [HttpGet]
        public IEnumerable<VariableViewModel> GetAllAnalytes()
        {
            var sites = _variableRepository.GetAllChemistryVariables().Where(x => x.VariableDefinition != null);
            var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
            return items;
        }

        [HttpPost]
        public IEnumerable<StationAnalyteQueryViewModel> FetchStationData(FetchSiteAnalyteQueryViewModel queryViewModel)
        {
            var items = new List<StationAnalyteQueryViewModel>();
            var actions = _wqDataRepository.GetAllWQAnalyteDataActions();

            foreach (var action in actions)
            {
                foreach (var analyte in queryViewModel.SelectedVariables)
                {
                    var result = action.FeatureActions.Where(x => x.SamplingFeatureID == queryViewModel.SelectedSiteID).FirstOrDefault().Results.Where(x => x.VariableID == analyte).FirstOrDefault();
                    if (result != null)
                    {
                        if (result.MeasurementResult != null && result.MeasurementResult.MeasurementResultValues.First().ValueDateTime <= queryViewModel.EndDate && result.MeasurementResult.MeasurementResultValues.First().ValueDateTime >= queryViewModel.StartDate)
                        {
                            var measurementValue = result.MeasurementResult.MeasurementResultValues.FirstOrDefault().DataValue;
                            var resultDateTime = action.BeginDateTime;
                            var unitsName = result.Unit.UnitsName;
                            var variable = result.Variable.VariableDefinition;
                            items.Add(new StationAnalyteQueryViewModel { DataValue = measurementValue, ResultDateTime = resultDateTime, UnitsName = unitsName, Variable = variable });
                        }
                    }
                }
            }
            return items;
        }
    }
}
