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
    public class PackageRepo : IPackageRepo
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public PackageRepo(ConstructionDbContext context, CustomLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PackageModel>> GetPackages(int accountId)
        {
            List<PackageModel> packages = new List<PackageModel>();
            try
            {
                _logger.Log("Packages.GetPackages Started.");
                packages = await (from p in _context.Packages.Where(p => p.AccountId == accountId)
                                  orderby p.PackageId
                                  select new PackageModel
                                  {
                                      PackageId = p.PackageId,
                                      PackageName = p.PackageName,
                                      Description = p.Description,
                                      PackagePrice = p.PackagePrice,
                                      AccountId = p.AccountId,
                                      ConstructionTypeId = p.ConstructionTypeId,
                                      ConstructionType = p.ConstructionType != null ? p.ConstructionType.ConstructionType : null,
                                      IsActive = p.IsActive,
                                      CreatedBy = p.CreatedBy,
                                      CreatedDate = p.CreatedDate,
                                      ModifiedBy = p.ModifiedBy,
                                      ModifiedDate = p.ModifiedDate,
                                      Features = p.PackageFeatures
                                          .Where(f => f.IsActive)
                                          .OrderBy(f => f.PackageFeatureId)
                                          .Select(f => new PackageFeatureModel
                                          {
                                              PackageFeatureId = f.PackageFeatureId,
                                              PackageId = f.PackageId,
                                              Feature = f.Feature,
                                              IsActive = f.IsActive
                                          }).ToList(),
                                      Specs = p.PackageSpecs
                                          .Where(s => s.IsActive)
                                          .OrderBy(s => s.PackageSpecId)
                                          .Select(s => new PackageSpecModel
                                          {
                                              PackageSpecId = s.PackageSpecId,
                                              PackageId = s.PackageId,
                                              SpecLabel = s.SpecLabel,
                                              SpecValue = s.SpecValue,
                                              SpecTypeId = s.SpecTypeId,
                                              IsActive = s.IsActive
                                          }).ToList()
                                  }).ToListAsync();
                _logger.Log("Packages.GetPackages Completed.");
                return packages;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ConstructionTypeModel>> GetConstructionTypes()
        {
            List<ConstructionTypeModel> types = new List<ConstructionTypeModel>();
            try
            {
                _logger.Log("Packages.GetConstructionTypes Started.");
                types = await (from t in _context.MasterConstructionTypes.Where(t => t.IsActive == true)
                               orderby t.ConstructionTypeId
                               select new ConstructionTypeModel
                               {
                                   ConstructionTypeId = t.ConstructionTypeId,
                                   ConstructionType = t.ConstructionType,
                                   Description = t.Description,
                                   IsActive = t.IsActive
                               }).ToListAsync();
                _logger.Log("Packages.GetConstructionTypes Completed.");
                return types;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PackageSpecTypeModel>> GetPackageSpecTypes()
        {
            List<PackageSpecTypeModel> types = new List<PackageSpecTypeModel>();
            try
            {
                _logger.Log("Packages.GetPackageSpecTypes Started.");
                types = await (from t in _context.MasterPackageSpecTypes.Where(t => t.IsActive == true)
                               orderby t.PackageSpecTypeId
                               select new PackageSpecTypeModel
                               {
                                   PackageSpecTypeId = t.PackageSpecTypeId,
                                   PackageSpecType = t.PackageSpecType,
                                   Description = t.Description,
                                   IsActive = t.IsActive
                               }).ToListAsync();
                _logger.Log("Packages.GetPackageSpecTypes Completed.");
                return types;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseMsg> SavePackage(PackageModel packageModel)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Packages.SavePackage Started.");
                if (packageModel != null)
                {
                    if (packageModel.PackageId > 0)
                    {
                        var selectedPackage = await _context.Packages
                            .Where(p => p.PackageId == packageModel.PackageId)
                            .FirstOrDefaultAsync();

                        if (selectedPackage != null)
                        {
                            selectedPackage.PackageName = packageModel.PackageName;
                            selectedPackage.Description = packageModel.Description;
                            selectedPackage.PackagePrice = packageModel.PackagePrice;
                            selectedPackage.ConstructionTypeId = packageModel.ConstructionTypeId;
                            selectedPackage.IsActive = packageModel.IsActive;
                            selectedPackage.ModifiedBy = packageModel.ModifiedBy;
                            selectedPackage.ModifiedDate = DateTime.Now;

                            await ReplaceFeaturesAsync(selectedPackage.PackageId, packageModel.Features);
                            await ReplaceSpecsAsync(selectedPackage.PackageId, packageModel.Specs);

                            responseMsg.Id = selectedPackage.PackageId;
                            responseMsg.Message = "Package Updated Successfully";
                        }
                        else
                        {
                            responseMsg.Message = "Package Not Found";
                            return responseMsg;
                        }
                    }
                    else
                    {
                        Package package = new Package
                        {
                            PackageName = packageModel.PackageName,
                            Description = packageModel.Description,
                            PackagePrice = packageModel.PackagePrice,
                            AccountId = packageModel.AccountId,
                            ConstructionTypeId = packageModel.ConstructionTypeId,
                            IsActive = packageModel.IsActive,
                            CreatedBy = packageModel.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        await _context.Packages.AddAsync(package);
                        await _context.SaveChangesAsync();

                        await ReplaceFeaturesAsync(package.PackageId, packageModel.Features);
                        await ReplaceSpecsAsync(package.PackageId, packageModel.Specs);

                        responseMsg.Id = package.PackageId;
                        responseMsg.Message = "Package Added Successfully";
                    }
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                }
                _logger.Log("Packages.SavePackage Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseMsg> DeletePackage(int packageId)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Packages.DeletePackage Started.");
                var selectedPackage = await _context.Packages
                    .Where(p => p.PackageId == packageId)
                    .FirstOrDefaultAsync();

                if (selectedPackage != null)
                {
                    var features = await _context.PackageFeatures
                        .Where(f => f.PackageId == packageId)
                        .ToListAsync();
                    if (features.Count > 0)
                    {
                        _context.PackageFeatures.RemoveRange(features);
                    }
                    var specs = await _context.PackageSpecs
                        .Where(s => s.PackageId == packageId)
                        .ToListAsync();
                    if (specs.Count > 0)
                    {
                        _context.PackageSpecs.RemoveRange(specs);
                    }
                    _context.Packages.Remove(selectedPackage);
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                    responseMsg.Id = packageId;
                    responseMsg.Message = "Package Deleted Successfully";
                }
                else
                {
                    responseMsg.Message = "Package Not Found";
                }
                _logger.Log("Packages.DeletePackage Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task ReplaceFeaturesAsync(int packageId, List<PackageFeatureModel> features)
        {
            if (features == null) return;

            var existing = await _context.PackageFeatures
                .Where(f => f.PackageId == packageId)
                .ToListAsync();

            if (existing.Count > 0)
            {
                _context.PackageFeatures.RemoveRange(existing);
                await _context.SaveChangesAsync();
            }

            var trimmed = features
                .Where(f => !string.IsNullOrWhiteSpace(f.Feature))
                .ToList();

            if (trimmed.Count == 0) return;

            foreach (var f in trimmed)
            {
                _context.PackageFeatures.Add(new PackageFeature
                {
                    PackageId = packageId,
                    Feature = f.Feature.Trim(),
                    IsActive = f.IsActive
                });
            }
        }

        private async Task ReplaceSpecsAsync(int packageId, List<PackageSpecModel> specs)
        {
            if (specs == null) return;

            var existing = await _context.PackageSpecs
                .Where(s => s.PackageId == packageId)
                .ToListAsync();

            if (existing.Count > 0)
            {
                _context.PackageSpecs.RemoveRange(existing);
                await _context.SaveChangesAsync();
            }

            var trimmed = specs
                .Where(s => !string.IsNullOrWhiteSpace(s.SpecLabel) && !string.IsNullOrWhiteSpace(s.SpecValue))
                .ToList();

            if (trimmed.Count == 0) return;

            foreach (var s in trimmed)
            {
                _context.PackageSpecs.Add(new PackageSpec
                {
                    PackageId = packageId,
                    SpecLabel = s.SpecLabel.Trim(),
                    SpecValue = s.SpecValue.Trim(),
                    SpecTypeId = s.SpecTypeId,
                    IsActive = s.IsActive
                });
            }
        }
    }
}