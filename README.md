| Requirement                                  | Why                                                |
| -------------------------------------------- | -------------------------------------------------- |
| Native EF Core provider                      | Application's code-first model stays authoritative |
| Native declarative object language           | Image contains provider-native objects             |
| Script/artifact generation without live DB   | Image is build-time, not convergence-time          |
| Fresh initialization from object definitions | Image is actually deployable                       |
| Application/system object distinction        | Image doesn't own provider internals               |
| Deterministic object representation          | OCI digest can identify logical DB state           |
| Cheap/new physical realization               | Makes replacement deployment practical             |
| Data can be established independently        | Object state ≠ data state                          |
| Introspection/verification                   | Prove realization matches image                    |
| OCI-compatible distribution possible         | First-class Amiasea artifact                       |
