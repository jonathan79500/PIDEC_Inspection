using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Documents;


// A DocumentDto is built from three sources: the document itself, the name of its type (type table)
// and the stored file's info (attachment store). This record bundles them so one Adapt call does the mapping.
public record DocumentView(Document Document, string DocumentTypeName, AttachmentInfo? Info);




