// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Utils;

public enum Scope {
    Read,
    Create,
    Index,
    ObjectPermissions,
    UserManagement,
    Update,
    Delete
}

public static class ScopeMapper {
    internal static Protobuf.Scope GetServiceScope(this Scope scope) {
        switch (scope) {
            case Scope.Read:
                return Protobuf.Scope.Read;
            case Scope.Create:
                return Protobuf.Scope.Create;
            case Scope.Index:
                return Protobuf.Scope.Index;
            case Scope.ObjectPermissions:
                return Protobuf.Scope.Objectpermissions;
            case Scope.UserManagement:
                return Protobuf.Scope.Usermanagement;
            case Scope.Update:
                return Protobuf.Scope.Update;
            case Scope.Delete:
                return Protobuf.Scope.Delete;
            default:
                throw new ArgumentException("Unsupported Scope");
        }
    }
}
