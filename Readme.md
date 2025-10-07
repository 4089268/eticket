# eTicket

A .NET project for managing tickets

## Getting Started

1. Clone the repository.
2. Restore dependencies with `dotnet restore`.
3. Update the database connection string as needed.



## Niveles de Usuarios

### Director General
Puede ver los reportes de las diferentes oficinas y cuenta con todos los privilegios, excepto los de administrador.

### Director de Oficina
Puede ver los reportes y tiene todos los privilegios respecto a la(s) oficinas asignadas.

### Usuario
Tiene ciertos privilegios respecto a la(s) oficinas asignadas.

### Capturista
Permite generar nuevos reportes y asignarlos a las oficinas correspondientes.