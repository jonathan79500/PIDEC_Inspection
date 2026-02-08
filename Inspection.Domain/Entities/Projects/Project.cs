using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspection.Domain.Entities.Projects;

public sealed class Project : AggregateRoot<ProjectId>
{
    public ProjectId? ProjectId { get; set; }
    public string? Title { get;  set; }
    public string? Description { get;  set; } 
    public string? Number { get;  set; }
    public string? FSId { get;  set; }//means Foreign System Id

    [NotMapped]
    public List<ProjectContact>? PidecQCContacts { get; set; }
    [NotMapped]
    public List<ProjectContact> OwnerContacts { get; set; }
    [NotMapped]
    public List<ProjectContact> MCContacts { get; set; }
    [NotMapped]
    public List<ProjectContact> PidecInspectors { get; set; } //Second Part Inspectors
    [NotMapped]
    public List<ProjectContact> TPInspectors { get; set; }//Third Part Inspectors

    [NotMapped]
    public List<InspectorProject> Inspectors { get; set; }
    //public List<ProjectContact> OwnerContacts { get; set; }
    //public List<ProjectContact> MCContacts { get; set; }
    //public List<ProjectContact> PidecInspectors { get; set; } //Second Part Inspectors
    //public List<ProjectContact> TPInspectors { get; set; }//Third Part Inspectors

    private readonly List<ProjectContact> PIDEC_TO = new();
    private readonly List<ProjectContact> PIDEC_CC = new();
    private readonly List<InspectionSubject> inspectionSubjects = new();
    //prjNo(4digit)-INS|NTF-Order No(3 digits)-Inspection Number(3 digits)-Rev(2digits(revision Numver))
    private string InspectionNumberPattern = @"(\d{4})-INS-(\d{3})-(\d{3})-(Rev\d{2})|(\d{4})-NTF-(\d{3})-(\d{3})-(Rev\d{2})";

    public List<Order>? Orders { get;  set; }
    
    public List<ProjectContact>? Contacts { get;  set; }  
    
    public Project() {
    
    }

    public Project(ProjectId id) : base(id)
    {
        this.ProjectId = id;
    }

    public static Project Create(Guid id, string title, string description, string number, string fsId)
    {
        return new Project(id)
        {
            Title = title,
            Description = description,
            Number = number,
            FSId = fsId
        };
    }

    public static Project Create(string title, string description, string number, string fsId)
    {
        return new Project()
        {
            Title = title,
            Description = description,
            Number = number,
            FSId = fsId
        };
    }
}
