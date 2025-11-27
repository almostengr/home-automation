using Microsoft.AspNetCore.Mvc;
using Almostengr.LcdDashboard.Models;
using Almostengr.Common.Controllers;
using Almostengr.Common.DomainServices.Results;
using Almostengr.LcdDashboard.Features.SystemSettings.DomainServices.Interfaces;

namespace Almostengr.LcdDashboard.Controllers;

public class SystemSettingsController : BaseUiController
{
    private readonly IAddSystemSettingsService _addService;
    private readonly IQuerySystemSettingService _queryService;
    private readonly IUpdateSystemSettingsService _updateService;

    public SystemSettingsController(
        IQuerySystemSettingService queryService,
        IAddSystemSettingsService addService,
        IUpdateSystemSettingsService updateService
    )
    {
        _addService = addService; 
        _queryService = queryService;
        _updateService = updateService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IList<SystemSettingEntity> settings = await _queryService.GetListAsync();
        return View(settings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SystemSettingViewModel model = new();
        // model.SystemSettingList = 
        // todo enum need to be available in drop down list
        return PartialView("_CreateEdit", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SystemSettingViewModel model)
    {
        if (ModelState.IsValid)
        {
            Result<SystemSettingViewModel> result = await _addService.ExecuteAsync(model);
            if (result.Succeeded)
            {
                return NoContent();
            }
            AddErrorsToModelState(result.Errors);
        }

        return PartialView("_CreateEdit", model);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        SystemSettingEntity entity = await _queryService.GetByOptionAsync(id);
        if (entity == null)
        {
            return NotFoundParitalView();
        }

        SystemSettingViewModel model = new(entity);
        return PartialView("_CreateEdit", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SystemSettingViewModel model)
    {
        if (ModelState.IsValid)
        {
            SystemSettingEntity entity = await _queryService.GetByOptionAsync((int)model.Option);
            if (entity == null)
            {
                return NotFoundParitalView();
            }

            Result<SystemSettingViewModel> result = await _updateService.ExecuteAsync(model);
            if (result.Succeeded)
            {
                return NoContent();
            }
            AddErrorsToModelState(result.Errors);
        }
        return PartialView("_CreateEdit", model);
    }
}
