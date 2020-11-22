﻿namespace FDS.Package.Service.Commands
{
    using MediatR;

    public class UpdatePackageVersionCommand : IRequest<Models.Package>
    {
        public UpdatePackageVersionCommand(int packageId, int versionId)
        {
            PackageId = packageId;
            VersionId = versionId;
        }

        public int PackageId { get; private set; }
        
        public int VersionId { get; private set; }
    }
}
