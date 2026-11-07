using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Entity.EntityClass;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSConstructions.Repository.Implementation
{
    public class ExtraRepo : IExtraRepo
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public ExtraRepo(ConstructionDbContext context, CustomLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ExtraModel>> GetExtras(int accountId)
        {
            List<ExtraModel> extras = new List<ExtraModel>();
            try
            {
                _logger.Log("Extras.GetExtras Started.");
                extras = await (from e in _context.Extras.Where(e => e.AccountId == accountId)
                                orderby e.ExtrasId
                                select new ExtraModel
                                {
                                    ExtrasId = e.ExtrasId,
                                    ExtrasName = e.ExtrasName,
                                    Description = e.Description,
                                    ExtrasPrice = e.ExtrasPrice,
                                    IsExtrasExtra = e.IsExtrasExtra,
                                    ExtrasExtrasPlaceHolder = e.ExtrasExtrasPlaceHolder,
                                    ExtrasExtrasUnit = e.ExtrasExtrasUnit,
                                    AccountId = e.AccountId,
                                    IsActive = e.IsActive,
                                    CreatedBy = e.CreatedBy,
                                    CreatedDate = e.CreatedDate,
                                    ModifiedBy = e.ModifiedBy,
                                    ModifiedDate = e.ModifiedDate
                                }).ToListAsync();
                _logger.Log("Extras.GetExtras Completed.");
                return extras;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseMsg> SaveExtra(ExtraModel extraModel)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Extras.SaveExtra Started.");
                if (extraModel != null)
                {
                    if (string.IsNullOrWhiteSpace(extraModel.ExtrasName))
                    {
                        responseMsg.Message = "Extras Name is Required";
                        return responseMsg;
                    }

                    if (extraModel.ExtrasId > 0)
                    {
                        var selectedExtra = await _context.Extras
                            .Where(e => e.ExtrasId == extraModel.ExtrasId)
                            .FirstOrDefaultAsync();

                        if (selectedExtra != null)
                        {
                            selectedExtra.ExtrasName = extraModel.ExtrasName.Trim();
                            selectedExtra.Description = extraModel.Description;
                            selectedExtra.ExtrasPrice = extraModel.ExtrasPrice;
                            selectedExtra.IsExtrasExtra = extraModel.IsExtrasExtra;
                            selectedExtra.ExtrasExtrasPlaceHolder = extraModel.ExtrasExtrasPlaceHolder;
                            selectedExtra.ExtrasExtrasUnit = extraModel.ExtrasExtrasUnit;
                            selectedExtra.IsActive = extraModel.IsActive;
                            selectedExtra.ModifiedBy = extraModel.ModifiedBy;
                            selectedExtra.ModifiedDate = DateTime.Now;

                            responseMsg.Id = selectedExtra.ExtrasId;
                            responseMsg.Message = "Extra Updated Successfully";
                        }
                        else
                        {
                            responseMsg.Message = "Extra Not Found";
                            return responseMsg;
                        }
                    }
                    else
                    {
                        Extra extra = new Extra
                        {
                            ExtrasName = extraModel.ExtrasName.Trim(),
                            Description = extraModel.Description,
                            ExtrasPrice = extraModel.ExtrasPrice,
                            IsExtrasExtra = extraModel.IsExtrasExtra,
                            ExtrasExtrasPlaceHolder = extraModel.ExtrasExtrasPlaceHolder,
                            ExtrasExtrasUnit = extraModel.ExtrasExtrasUnit,
                            AccountId = extraModel.AccountId,
                            IsActive = extraModel.IsActive,
                            CreatedBy = extraModel.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        await _context.Extras.AddAsync(extra);
                        await _context.SaveChangesAsync();

                        responseMsg.Id = extra.ExtrasId;
                        responseMsg.Message = "Extra Added Successfully";
                    }
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                }
                _logger.Log("Extras.SaveExtra Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseMsg> DeleteExtra(int extrasId)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Extras.DeleteExtra Started.");
                var selectedExtra = await _context.Extras
                    .Where(e => e.ExtrasId == extrasId)
                    .FirstOrDefaultAsync();

                if (selectedExtra != null)
                {
                    _context.Extras.Remove(selectedExtra);
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                    responseMsg.Id = extrasId;
                    responseMsg.Message = "Extra Deleted Successfully";
                }
                else
                {
                    responseMsg.Message = "Extra Not Found";
                }
                _logger.Log("Extras.DeleteExtra Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
