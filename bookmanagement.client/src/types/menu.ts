export type PermissionType = "View" | "Create" | "Update" | "Delete" | "Import";

export interface MenuResponse {
  id: string
  name: string
  url: string
  parentId: string | null
  isVisible: boolean
  permissionTypes: PermissionType[]
  children: MenuResponse[]
  permissions: unknown[]
}
