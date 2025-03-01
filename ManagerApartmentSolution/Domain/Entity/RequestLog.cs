﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerApartment.Models;

public partial class RequestLog
{
    public int RequestLogId { get; set; }

    public int? RequestId { get; set; }

    public string? MaintainItem { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    [JsonInclude] public virtual Request? Request { get; set; }

    [JsonInclude] public virtual ICollection<StaffLog> StaffLogs { get; set; } = new List<StaffLog>();
}
