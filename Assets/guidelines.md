# Unity Game Development Guidelines

## Code Standards

### Logging and Comments
- **Debug logs**: Only add debug logs when refactoring or debugging specific issues
- **Comments**: Only add comments when explicitly requested

### Input Handling
- Avoid Unity's Event system for input implementation
- Use direct input handling methods instead

### Dependency Injection with Zenject
- **Never instantiate services directly** (e.g., `ExampleService = new ExampleService()`)
- **Global services**: Bind in `InfrastructureInstaller`
- **Scene-specific services**: Bind in the respective scene installer
- **Injection methods**:
  - Plain C# classes: Pass dependencies through constructor
  - MonoBehaviour classes: Use custom `Construct()` method with `[Inject]` attribute

### Code Structure
- **Field/property placement**: Declare all fields and properties at the top of the class
- **MonoBehaviour initialization**: Use `Construct()` method only for setting dependencies, not game logic

## Data Persistence System

### Save Data Architecture
- All save data must be contained within `PersistentPlayerProgress`
- `PlayerProgress` class implements all data that needs to be saved
- For specific data types (e.g., level data), create serializable classes like `LevelData` with necessary fields

### Save/Load Process
1. **Register objects** in the existing `SaveLoadService`
2. **Implement interfaces**:
  - `IProgressUpdater`: For objects that need to update their data
  - `IProgressSaver`: For objects that need to save their data
3. **Save data**: Call `SaveProgress()` method in `SaveLoadService` (triggers all registered `IProgressSavers`)
4. **Update data**: Call `UpdateProgress()` method (triggers all registered `IProgressUpdaters`)
5. **Direct data access**: Inject `PersistentPlayerProgress` as dependency for direct data retrieval without triggering all updaters

### Static Data Management
- Create appropriate config files for all static data
- Load configs into `StaticDataService`
- Access static data through `StaticDataService`

## Refactoring Guidelines

### What NOT to Change
- **No renaming** unless explicitly requested
- **No if-statement inversion** during refactoring
- **No readability attributes** (like `[Header]`) unless requested

### Code Formatting
- **Method spacing**: Maintain at least two blank lines between methods