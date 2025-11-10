using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class VendorRepository : IVendorRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenericRepository<Vendor>> _logger;

        public VendorRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Vendor>> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public async Task<List<VendorMaterialDetailsDto>> GetVendorsAsync(int vendorId)
        {
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            const string sql = "SELECT * FROM vendor.get_vendor_and_material_details(@VendorId)";
            var result = await connection.QueryAsync<VendorMaterialDetailsDto>(sql, new { VendorId = vendorId });

            return result.ToList(); // Return just the list
        }
        public async Task<BaseResponse> UpsertPurchaseOrderAsync(PurchaseOrderDto? dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_purchase_order_id", dto.PurchaseOrderId != 0 ? dto.PurchaseOrderId : null, DbType.Int32);
                parameters.Add("p_po_id", string.IsNullOrWhiteSpace(dto.PoId) ? null : dto.PoId, DbType.String);
                parameters.Add("p_boq_code", string.IsNullOrWhiteSpace(dto.BoqCode) ? null : dto.BoqCode, DbType.String);
                parameters.Add("p_created_by", dto.CreatedBy != 0 ? dto.CreatedBy : null, DbType.Int32);
                parameters.Add("p_status", string.IsNullOrWhiteSpace(dto.status) ? null : dto.status, DbType.String);
                parameters.Add("p_delivery_status",
    string.IsNullOrWhiteSpace(dto.DeliveryStatus) ? (object)DBNull.Value : dto.DeliveryStatus,
    DbType.String);

                parameters.Add("p_delivery_status_date",
                    dto.DeliveryStatusDate.HasValue ? dto.DeliveryStatusDate : (object)DBNull.Value,
                    DbType.DateTime);
                //parameters.Add("p_delivery_status", string.IsNullOrWhiteSpace(dto.DeliveryStatus) ? null : dto.status, DbType.String);

                //parameters.Add("p_delivery_status_date", dto.DeliveryStatusDate.HasValue ? dto.DeliveryStatusDate : (object)DBNull.Value, DbType.DateTime);


                // Format items with correct property names matching PostgreSQL expected format
                if (dto.Items == null || !dto.Items.Any())
                {
                    parameters.Add("p_items_data", null, DbType.Object);
                }
                else
                {
                    var formattedItems = dto.Items.Select(x => new {
                        purchase_order_items_id = x?.ItemId == 0 ? (int?)null : x?.ItemId,
                        item_name = x?.ItemName ?? "",
                        unit = x?.Unit ?? "",
                        price = x?.Price ?? 0,
                        quantity = x?.Quantity ?? 0,
                        total = x?.Total ?? 0
                    });
                    parameters.Add("p_items_data", JsonConvert.SerializeObject(formattedItems), DbType.Object);
                }

                // Assign to array (nullable)
                parameters.Add("p_assign_to", dto.AssignTo?.ToArray() ?? null, DbType.Object);
                // Ticket type
                parameters.Add("p_ticket_type", string.IsNullOrWhiteSpace(dto.TicketType) ? null : dto.TicketType, DbType.String);
                // OUT parameters
                parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
                parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 20);
                parameters.Add("out_po_id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                // Call stored procedure
                await con.ExecuteAsync(@"
    CALL vendor.upsert_purchase_order_with_status(
        @p_purchase_order_id,
        @p_po_id,
        @p_boq_code,
        @p_created_by,
        @p_status,
@p_delivery_status,
@p_delivery_status_date,
        @p_items_data::jsonb,
        @p_assign_to,
        @p_ticket_type,
        @message,
        @status,
        @out_po_id
    );
", parameters);

                var status = parameters.Get<string>("status");
                var message = parameters.Get<string>("message");
                var outPoId = parameters.Get<int>("out_po_id");

                response.Success = status?.ToLower() == "success";
                response.Message = message;
                response.Data = new { PurchaseOrderId = outPoId };
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Error in UpsertPurchaseOrderAsync: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while saving the purchase order";
                response.Data = false;
            }
            return response;
        }

        //        public async Task<BaseResponse> UpsertPurchaseOrderAsync(PurchaseOrderDto? dto)
        //        {
        //            using var con = CreateConnection();
        //            var response = new BaseResponse();

        //            try
        //            {
        //                var parameters = new DynamicParameters();

        //                parameters.Add("p_purchase_order_id", dto.PurchaseOrderId != 0 ? dto.PurchaseOrderId : null, DbType.Int32);
        //                parameters.Add("p_po_id", string.IsNullOrWhiteSpace(dto.PoId) ? null : dto.PoId, DbType.String);
        //                parameters.Add("p_boq_code", string.IsNullOrWhiteSpace(dto.BoqCode) ? null : dto.BoqCode, DbType.String);
        //                parameters.Add("p_created_by", dto.CreatedBy != 0 ? dto.CreatedBy : null, DbType.Int32);
        //                parameters.Add("p_status", string.IsNullOrWhiteSpace(dto.status) ? null : dto.status, DbType.String);

        //                // Handle null or empty items
        //                if (dto.Items == null || !dto.Items.Any())
        //                {
        //                    parameters.Add("p_items_data", null, DbType.Object);
        //                }
        //                else
        //                {
        //                    var formattedItems = dto.Items.Select(x => new {
        //                        item_name = x?.ItemName ?? "",
        //                        unit = x?.Unit ?? "",
        //                        price = x?.Price ?? 0,
        //                        quantity = x?.Quantity ?? 0,
        //                        total = x?.Total ?? 0
        //                    });

        //                    parameters.Add("p_items_data", JsonConvert.SerializeObject(formattedItems), DbType.Object);
        //                }

        //                // Assign to array (handle null)
        //                parameters.Add("p_assign_to", dto.AssignTo?.ToArray() ?? null, DbType.Object);

        //                // Ticket type
        //                parameters.Add("p_ticket_type", string.IsNullOrWhiteSpace(dto.TicketType) ? null : dto.TicketType, DbType.String);

        //                // OUT parameters
        //                parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
        //                parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 20);
        //                parameters.Add("out_po_id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

        //                //var parameters = new DynamicParameters();

        //                //parameters.Add("p_purchase_order_id", dto.PurchaseOrderId, DbType.Int32); // NULL for insert
        //                //parameters.Add("p_po_id", dto.PoId, DbType.String);
        //                //parameters.Add("p_boq_code", dto.BoqCode, DbType.String);
        //                //parameters.Add("p_created_by", dto.CreatedBy, DbType.Int32);
        //                //parameters.Add("p_status", dto.status, DbType.String);

        //                //// Serialize item list to JSONB string

        //                //var formattedItems = dto.Items.Select(x => new {
        //                //    //boq_items_id = x.BoqItemsId,
        //                //    item_name = x.ItemName??"",
        //                //    unit = x.Unit??"",
        //                //    price = x.Price??0,
        //                //    quantity = x.Quantity??0,
        //                //    total= x.Total??0,
        //                //});

        //                //parameters.Add("p_items_data", JsonConvert.SerializeObject(formattedItems), DbType.Object);
        //                ////parameters.Add("p_items_data", JsonConvert.SerializeObject(dto.Items), DbType.String);

        //                //// Convert assign_to list to integer[]
        //                //parameters.Add("p_assign_to", dto.AssignTo.ToArray(), DbType.Object);

        //                //parameters.Add("p_ticket_type", dto.TicketType, DbType.String);

        //                //// OUT parameters
        //                //parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
        //                //parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 20);
        //                //parameters.Add("out_po_id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

        //                // Call the stored procedure
        //                await con.ExecuteAsync(@"
        //     CALL vendor.upsert_purchase_order_with_boq_code(
        //         @p_purchase_order_id,
        //         @p_po_id,
        //         @p_boq_code,
        //         @p_created_by,
        //@p_status,
        //         @p_items_data::jsonb,
        //         @p_assign_to,
        //         @p_ticket_type,
        //         @message,
        //         @status,
        //         @out_po_id
        //     );
        // ", parameters);

        //                var status = parameters.Get<string>("status");
        //                var message = parameters.Get<string>("message");
        //                var outPoId = parameters.Get<int>("out_po_id");

        //                response.Success = status?.ToLower() == "success";
        //                response.Message = message;
        //                response.Data = new { PurchaseOrderId = outPoId };
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError("❌ Error in UpsertPurchaseOrderAsync: " + ex.Message);
        //                response.Success = false;
        //                response.Message = "An error occurred while saving the purchase order: " + ex.Message;
        //                response.Data = false;
        //            }

        //            return response;
        //        }

        public async Task<List<VendorMaterialDetailsDto>> GetVendorAndMaterialDetailsAsync(int vendorId)
        {
            using var connection = (NpgsqlConnection)CreateConnection();
            await connection.OpenAsync();

            var result = await connection.QueryAsync<dynamic>(
                "SELECT * FROM vendor.get_vendor_and_material_details(@VendorId)",
                new { VendorId = vendorId });

            if (!result.Any())
                return new List<VendorMaterialDetailsDto>();

            var materials = result.Select(r => new MaterialDetailDto
            {
                MaterialId = r.materialid,
                MaterialName = r.materialname,
                Unit = r.unit,
                CurrentPrice = r.currentprice,
                Price = r.price,
                MaterialCategoryId = r.materialcategoryid,
                MaterialCategoryName = r.materialcategoryname
            }).ToList();

            var first = result.First();

            var vendorDto = new VendorMaterialDetailsDto
            {
                VendorId = first.vendor_id,
                VendorName = first.vendorname,
                ContactNumber = first.contactnumber,
                Email = first.email,
                Materials = materials
            };

            return new List<VendorMaterialDetailsDto> { vendorDto };

        }


        public async Task<List<PurchaseOrderDetailsDto>> GetPurchaseOrderDetailsAsync(int purchase_order_id)
        {
            try
            {
                using var connection = (NpgsqlConnection)CreateConnection();
                await connection.OpenAsync();
                var sql = "SELECT * FROM vendor.get_purchase_order_details1(@purchase_order_id);";
                var result = (await connection.QueryAsync<dynamic>(sql, new { purchase_order_id })).ToList();

                if (!result.Any())
                    return new List<PurchaseOrderDetailsDto>();

                var purchaseOrderItems = result.Select(r => new PurchaseOrderItemDetailDto
                {
                    PurchaseOrderItemsId = r.purchase_order_items_id,
                    ItemName = r.item_name,
                    Unit = r.unit,
                    Price = r.price,
                    Quantity = r.quantity,
                    Total = r.total
                }).ToList();

                var first = result.First();

                // Parse the JSON field `approvers` with proper property name matching
                var approvers = new List<ApproverDto>();
                if (first.approvers != null)
                {
                    // Make sure property names match between JSON and ApproverDto
                    approvers = JsonConvert.DeserializeObject<List<ApproverDto>>(first.approvers.ToString());
                }

                var purchaseOrderDto = new PurchaseOrderDetailsDto
                {
                    PurchaseOrderId = first.purchase_order_id,
                    PoId = first.po_id,
                    BoqId = first.boq_id,
                    BoqCode = first.boq_code,
                    BoqName = first.boq_name,
                    VendorId = first.vendor_id,
                    VendorName = first.vendor_name,
                    ProjectId = first.project_id,
                    ProjectName = first.project_name,
                    Status=first.status,
                    DeliveryStatus = first.delivery_status,
                    DeliveryStatusDate= first.delivery_status_date,
                    PurchaseOrderItems = purchaseOrderItems,
                    Approvers = approvers
                };

                return new List<PurchaseOrderDetailsDto> { purchaseOrderDto };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get purchase order details.");
                return new List<PurchaseOrderDetailsDto>();
            }
        }
        //public async Task<List<PurchaseOrderDetailsDto>> GetPurchaseOrderDetailsAsync(int purchase_order_id)
        //{
        //    try
        //    {
        //        using var connection = (NpgsqlConnection)CreateConnection();
        //        await connection.OpenAsync();

        //        var sql = "SELECT * FROM vendor.get_purchase_order_details(@purchase_order_id);";

        //        var result = await connection.QueryAsync<dynamic>(sql, new { purchase_order_id });

        //        if (!result.Any())
        //            return new List<PurchaseOrderDetailsDto>();

        //        var purchaseOrderItems = result.Select(r => new PurchaseOrderItemDetailDto
        //        {
        //            PurchaseOrderItemsId = r.purchase_order_items_id,
        //            ItemName = r.item_name,
        //            Unit = r.unit,
        //            Price = r.price,
        //            Quantity = r.quantity,
        //            Total = r.total
        //        }).ToList();

        //        var first = result.First();

        //        var purchaseOrderDto = new PurchaseOrderDetailsDto
        //        {
        //            PurchaseOrderId = first.purchase_order_id,
        //            PoId = first.po_id,
        //            BoqId = first.boq_id,
        //            BoqName = first.boq_name,
        //            ProjectId = first.project_id,
        //            ProjectName = first.project_name,
        //            PurchaseOrderItems = purchaseOrderItems
        //        };

        //        return new List<PurchaseOrderDetailsDto> { purchaseOrderDto };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: log the error
        //        _logger.LogError(ex, "Failed to get purchase order details.");

        //        // Return empty list or rethrow depending on your API behavior
        //        return new List<PurchaseOrderDetailsDto>();
        //    }
        //}



        public async Task<List<BoqPurchaseOrderDetailsDto>> GetPurchaseOrdersByBoqCodeAsync(string boqCode)
        {
            try
            {
                using var connection = (NpgsqlConnection)CreateConnection();
                await connection.OpenAsync();

                var sql = "SELECT * FROM vendor.get_purchase_orders_by_boq_code(@p_boq_code);";

                var result = await connection.QueryAsync<dynamic>(sql, new { p_boq_code = boqCode });

                if (!result.Any())
                    return new List<BoqPurchaseOrderDetailsDto>();

                var first = result.First();

                var allItems = result.Select(r => new BoqPurchaseOrderItemDto
                {
                    PurchaseOrderItemsId = r.purchase_order_items_id,
                    ItemName = r.item_name,
                    Quantity = r.quantity,
                    Unit = r.unit,
                    Price = r.price,
                    Total = r.total
                }).ToList();

                var combined = new BoqPurchaseOrderDetailsDto
                {
                    BoqId = first.boq_id,
                    BoqCode = first.boq_code,
                    BoqName = first.boq_name,
                    PurchaseOrderId = first.purchase_order_id,
                    PoId = first.po_id,
                    VendorId = first.vendor_id,
                    VendorName = first.vendor_name,
                    VendorEmail = first.vendor_email,
                    VendorMobile = first.vendor_mobile,
                    PurchaseOrderItems = allItems
                };

                return new List<BoqPurchaseOrderDetailsDto> { combined };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get BOQ purchase order items.");
                return new List<BoqPurchaseOrderDetailsDto>();
            }
        }

        public async Task<List<PurchaseOrderWithItemsDto>> GetApprovedPurchaseOrdersWithItemsAsync(int vendorId)
        {
            var purchaseOrders = new Dictionary<int, PurchaseOrderWithItemsDto>();

            try
            {
                using var connection = (NpgsqlConnection)CreateConnection();
                string query = "SELECT * FROM vendor.get_approved_purchase_orders_with_items2(@vendor_id);";
                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("vendor_id", vendorId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int purchaseOrderId = reader.GetInt32(0);

                    if (!purchaseOrders.TryGetValue(purchaseOrderId, out var po))
                    {
                        po = new PurchaseOrderWithItemsDto
                        {
                            PurchaseOrderId = purchaseOrderId,
                            PoId = reader.GetString(1),
                            BoqId = reader.GetInt32(2),
                            CreatedBy = reader.GetInt32(3),
                            CreatedAt = reader.GetDateTime(4),
                            UpdatedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                            Status = reader.GetString(6),
                            DeliveryStatus = reader.IsDBNull(7) ? null : reader.GetString(7),
                            DeliveryStatusDate = reader.IsDBNull(8) ? null:reader.GetDateTime(8),
                            POReceivedDate = reader.GetDateTime(9),
                            VendorId = reader.GetInt32(10),
                            VendorMobile = reader.GetString(11),
                            PurchaseManagerMobile = reader.GetString(12),
                            ProjectName = reader.IsDBNull(13) ? null : reader.GetString(13),
                        };
                        purchaseOrders[purchaseOrderId] = po;
                    }

                    if (!reader.IsDBNull(14))
                    {
                        var item = new PurchaseOrderItemDto
                        {
                            PurchaseOrderItemsId = reader.GetInt32(14),
                            ItemName = reader.IsDBNull(15) ? null : reader.GetString(15),
                            Unit = reader.IsDBNull(16) ? null : reader.GetString(16),
                            Price = reader.IsDBNull(17) ? 0 : reader.GetDecimal(17),
                            Quantity = reader.IsDBNull(18) ? 0 : reader.GetInt32(18),
                            Total = reader.IsDBNull(19) ? 0 : reader.GetDouble(19)
                        };
                        po.Items.Add(item);
                    }
                }

                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return purchaseOrders.Values.ToList();
        }

        //public async Task<List<PurchaseOrderWithItemsDto>> GetApprovedPurchaseOrdersWithItemsAsync(int vendorId)
        //{
        //    var ordersDict = new Dictionary<int, PurchaseOrderWithItemsDto>();

        //    try
        //    {
        //        using var connection = (NpgsqlConnection)CreateConnection();
        //        string query = "SELECT * FROM vendor.get_approved_purchase_orders_with_items2(@vendor_id);";
        //        await connection.OpenAsync();

        //        using var cmd = new NpgsqlCommand(query, connection);
        //        cmd.Parameters.AddWithValue("vendor_id", vendorId);

        //        using var reader = await cmd.ExecuteReaderAsync();
        //        while (await reader.ReadAsync())
        //        {
        //            try
        //            {
        //                int purchaseOrderId = reader.GetInt32(0);

        //                if (!ordersDict.TryGetValue(purchaseOrderId, out var order))
        //                {
        //                    order = new PurchaseOrderWithItemsDto
        //                    {

        //                        PurchaseOrderId = purchaseOrderId,
        //                        PoId = reader.GetString(1),
        //                        BoqId = reader.GetInt32(2),
        //                        CreatedBy = reader.GetInt32(3),
        //                        CreatedAt = reader.GetDateTime(4),
        //                        UpdatedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
        //                        Status = reader.GetString(6),
        //                        DeliveryStatus = reader.GetString(7),
        //                        DeliveryStatusDate = reader.GetDateTime(8),
        //                        POReceivedDate = reader.GetDateTime(9),
        //                        VendorId = reader.GetInt32(10),
        //                        VendorMobile = reader.GetString(11),
        //                        PurchaseManagerMobile = reader.GetString(12),
        //                        ProjectName = reader.IsDBNull(13) ? null : reader.GetString(13),
        //                        Items = new List<PurchaseOrderItemDto>()
        //                    };
        //                    ordersDict[purchaseOrderId] = order;
        //                }

        //                // Add item if exists
        //                if (!reader.IsDBNull(14))
        //                {
        //                    order.Items.Add(new PurchaseOrderItemDto
        //                    {
        //                        PurchaseOrderItemsId = reader.GetInt32(14),
        //                        ItemName = reader.IsDBNull(15) ? null : reader.GetString(15),
        //                        Unit = reader.IsDBNull(16) ? null : reader.GetString(16),
        //                        Price = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
        //                        Quantity = reader.IsDBNull(18) ? null : reader.GetInt32(18),
        //                        Total = reader.IsDBNull(19) ? null : reader.GetDouble(19)
        //                    });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log per-row errors (optional)
        //                Console.WriteLine($"Row parsing error: {ex.Message}");
        //                // Optionally continue processing next row
        //            }
        //        }

        //        await connection.CloseAsync();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        Console.WriteLine($"Database error: {ex.Message}");
        //        // Optionally throw a custom exception or return an empty list
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Unexpected error: {ex.Message}");
        //        // Optionally rethrow or return a fallback result
        //    }

        //    return ordersDict.Values.ToList();
        //}


        //public async Task<List<PurchaseOrderDetailsDto>> GetPurchaseOrderDetailsAsync(int purchase_order_id)
        //{
        //    try
        //    {
        //        using var connection = (NpgsqlConnection)CreateConnection();
        //        await connection.OpenAsync();

        //        var sql = "SELECT * FROM vendor.get_purchase_order_details(@purchase_order_id);";

        //        var result = await connection.QueryAsync<dynamic>(sql, new { purchase_order_id });

        //        if (!result.Any())
        //            return new List<PurchaseOrderDetailsDto>();

        //        var purchaseOrderItems = result.Select(r => new PurchaseOrderItemDetailDto
        //        {
        //            PurchaseOrderItemsId = r.purchase_order_items_id,
        //            ItemName = r.item_name,
        //            Unit = r.unit,
        //            Price = r.price,
        //            Quantity = r.quantity,
        //            Total = r.total
        //        }).ToList();

        //        var first = result.First();

        //        var purchaseOrderDto = new PurchaseOrderDetailsDto
        //        {
        //            PurchaseOrderId = first.purchase_order_id,
        //            PoId = first.po_id,
        //            BoqId = first.boq_id,
        //            BoqName = first.boq_name,
        //            ProjectId = first.project_id,
        //            ProjectName = first.project_name,
        //            PurchaseOrderItems = purchaseOrderItems
        //        };

        //        return new List<PurchaseOrderDetailsDto> { purchaseOrderDto };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: log the error
        //        _logger.LogError(ex, "Failed to get purchase order details.");

        //        // Return empty list or rethrow depending on your API behavior
        //        return new List<PurchaseOrderDetailsDto>();
        //    }
        //}

        public async Task<List<PurchaseOrderDetailsData>> GetAllPurchaseOrderDetailsAsync()
        {
            try
            {
                using var connection = (NpgsqlConnection)CreateConnection();
                await connection.OpenAsync();

                var sql = "SELECT * FROM vendor.get_all_purchase_order_details();";

                var result = await connection.QueryAsync<dynamic>(sql);

                if (!result.Any())
                    return new List<PurchaseOrderDetailsData>();

                // Group by purchase order to handle multiple items per order
                var grouped = result.GroupBy(r => r.purchase_order_id);

                var purchaseOrders = grouped.Select(group =>
                {
                    var items = group.Select(r => new PurchaseOrderItemDetailDto
                    {
                        PurchaseOrderItemsId = r.purchase_order_items_id,
                        ItemName = r.item_name,
                        Unit = r.unit,
                        Price = r.price,
                        Quantity = r.quantity,
                        Total = r.total,
                        
                    }).ToList();

                    var first = group.First();

                    return new PurchaseOrderDetailsData
                    {
                        PurchaseOrderId = first.purchase_order_id,
                        PoId = first.po_id,
                        BoqId = first.boq_id,
                        ProjectName = first.project_name,
                        CreatedAt = first.created_at,
                        DeliveryStatus = first.delivery_status,
                        DeliveryStatusDate = first.delivery_status_date,
                        Vendor = first.vendor,
                        VendorMobileNumber = first.vendor_mobile_number,

                        PurchaseOrderItems = items,

                    };
                }).ToList();

                return purchaseOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all purchase order details.");
                return new List<PurchaseOrderDetailsData>();
            }
        }
    }
}
    



