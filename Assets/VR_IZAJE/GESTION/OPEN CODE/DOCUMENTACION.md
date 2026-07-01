# VR IZAJE — Documentación del Proyecto

Aplicación VR para visualización interactiva de componentes de vehículos de construcción.
Experiencia guiada por robot asistente con selección, rotación y exploración de piezas.

**Motor:** Unity 2022.3+ · **Target:** Oculus Quest / PC VR · **Pipeline:** Built-in
**Paquetes:** XR Interaction Toolkit 3.x, XR Core Utils, TextMeshPro

---

## Estructura del proyecto

```
Assets/VR_IZAJE/
├── CODIGO/
│   ├── Interacciones/      Button3D, SimpleGrab, SwitchBoton, SistemaAutopartes,
│   │                         RotacionArrastre, CambioMaterialHover,
│   │                         DestructorPorTrigger, CuentaRegresiva
│   ├── Sistema_pasos/      CondTiempo, CondBoton
│   ├── Animacion/          OscilacionOpacidad, OscilacionEmisivo, LineaEntrePuntos,
│   │                         NPCPatrol, LipSyncController
│   ├── Robot/              DialogData, DialogPlayer
│   ├── VR/                 VRHandController, DesktopInputController
│   └── Otros/              FechaTexto
├── ESCENAS/
│   ├── HOME.unity
│   ├── LOBBI.unity
│   ├── TRABAJO DE CAMPO IZAJE.unity
│   └── VEHICULOS DE CONSTRUCCION.unity
├── 3D/                     Modelos, animaciones, materiales, HDRI
├── PREFABS/                Prefabs estructurales, medianos, decorativos, interactivos
├── SONIDO/                 Archivos de audio
└── GESTION/                Documentación, reportes, notas
    ├── DAVID IZAJE/        Notas de contenido, guías, progreso
    ├── JUAN IZAJE/         Espacio de trabajo de Juan
    └── OPEN CODE/          Documentación, índice, reportes
```

---

## Scripts del sistema

### Interacciones (nuevo sistema modular)

| Script | Ruta | Función |
|--------|------|---------|
| Button3D | `CODIGO/Interacciones/Button3D.cs` | Botón 3D con hover/press/release y audio externo |
| SimpleGrab | `CODIGO/Interacciones/SimpleGrab.cs` | Agarre simplificado con retorno a posición base |
| SwitchBoton | `CODIGO/Interacciones/SwitchBoton.cs` | Toggle con listas de objetos modo interior/exterior |
| SistemaAutopartes | `CODIGO/Interacciones/SistemaAutopartes.cs` | Selección de pieza con cambio de padre, transición suave y toggle de objetos |
| RotacionArrastre | `CODIGO/Interacciones/RotacionArrastre.cs` | Rotación por arrastre con láser VR (trackball), sensibilidad configurable |
| CambioMaterialHover | `CODIGO/Interacciones/CambioMaterialHover.cs` | Feedback visual: cambia material al hacer hover con el láser |
| DestructorPorTrigger | `CODIGO/Interacciones/DestructorPorTrigger.cs` | Destruye objetos al entrar en trigger, reproduce sonido, activa objetos |
| CuentaRegresiva | `CODIGO/Interacciones/CuentaRegresiva.cs` | Desactiva el objeto al terminar cuenta regresiva |
| SistemaDeMatch | `CODIGO/Interacciones/SistemaDeMatch.cs` | Al entrar un objeto, desactiva/destruye ese objeto y activa su pareja. Al completar todos los pares, avanza el diálogo |

### Sistema de pasos y diálogos

| Script | Ruta | Función |
|--------|------|---------|
| DialogData | `CODIGO/Robot/DialogData.cs` | Datos del diálogo: texto, audio, animaciones, listas de objetos |
| DialogPlayer | `CODIGO/Robot/DialogPlayer.cs` | Reproductor de diálogos con timeline, lip sync, auto-avance |
| CondTiempo | `CODIGO/Sistema_pasos/CondTiempo.cs` | Avanza al siguiente diálogo tras un tiempo |
| CondBoton | `CODIGO/Sistema_pasos/CondBoton.cs` | Avanza al presionar un botón |
| CondTrigger | `CODIGO/Sistema_pasos/CondTrigger.cs` | Avanza al entrar N objetos mínimos en un trigger |
| CondSeleccion | `CODIGO/Sistema_pasos/CondSeleccion.cs` | Avanza al seleccionar N objetos distintos con SimpleGrab |

### Animación

| Script | Ruta | Función |
|--------|------|---------|
| OscilacionOpacidad | `CODIGO/Animacion/OscilacionOpacidad.cs` | Oscila transparencia de material |
| OscilacionEmisivo | `CODIGO/Animacion/OscilacionEmisivo.cs` | Oscila intensidad emisiva con color |
| LineaEntrePuntos | `CODIGO/Animacion/LineaEntrePuntos.cs` | LineRenderer entre dos transforms |

### VR

| Script | Ruta | Función |
|--------|------|---------|
| VRHandController | `CODIGO/VR/VRHandController.cs` | Láser con mirilla, detecta SimpleGrab, Button3D, RotacionArrastre, CambioMaterialHover, SistemaAutopartes |
| DesktopInputController | `CODIGO/VR/DesktopInputController.cs` | Modo escritorio (tecla P) |

---

## Flujo de selección VR (prioridad de detección)

VRHandController.HandleGrab() detecta componentes en este orden:
1. **SimpleGrab** → agarre del objeto
2. **RotacionArrastre** → rotación por arrastre
3. **Button3D** → botón clickeable (hover/press/release)
4. **SistemaAutopartes** → toggle de modo al soltar gatillo

**CambioMaterialHover** se detecta de forma independiente (funciona junto con cualquier componente).

---

## Flujo de diálogos

1. DialogPlayer.Start() activa `dialogList[0]`
2. Al activarse, DialogData.OnEnable() llama a DialogPlayer.Play()
3. Play() reproduce audio, texto sincronizado, animaciones, timeline
4. Al terminar: procesa objetosToActivateOnEnd/DeactivateOnEnd
5. Si autoAvanzar=true, llama a Avanzar() → desactiva actual, activa siguiente
6. CondTiempo / CondBoton llaman a Avanzar() para avanzar externamente

---

## Estado del proyecto (25 Junio 2026)

### Completado
- [x] Interacción base VR (láser, agarre, rotación, botones)
- [x] Sistema de diálogos con robot guía
- [x] Feedback visual hover con cambio de material
- [x] SistemaAutopartes: selección de piezas con transición y toggle
- [x] RotacionArrastre: rotación tipo trackball con láser
- [x] CambioMaterialHover: cambio de material en hijos recursivo
- [x] DestructorPorTrigger + CuentaRegresiva
- [x] Documentación y plan de proyecto actualizados

### Pendiente — Vehículos de Construcción
- [ ] Modelado 3D del camión y componentes (Fase 1)
- [ ] Configurar SistemaAutopartes en cada pieza
- [ ] Grabar audios del robot para cada componente
- [ ] Escribir textos de diálogo por pieza
- [ ] Pruebas de flujo completo en VR
- [ ] Build para PC VR / Quest

---

## Para comenzar

1. Abrir proyecto en Unity 2022.3+
2. Ir a `ESCENAS/VEHICULOS DE CONSTRUCCION.unity`
3. Presionar P durante Play para modo escritorio
4. Leer scripts en `CODIGO/Interacciones/` y `CODIGO/Robot/`
5. Agregar CambioMaterialHover a objetos para feedback visual hover
6. Configurar diálogos en DialogPlayer.dialogList

### Atajos de teclado (modo pruebas)
- **E** durante un diálogo → Salta el audio actual y ejecuta las acciones de finalización (activa/desactiva objetos, invoca eventos, auto-avanza si está configurado)

### Notas
- SistemaAutopartes requiere: parteSeleccionada, padreInicial, padreObservacion
- RotacionArrastre requiere: SphereCollider en el objeto
- CambioMaterialHover busca MeshRenderers en Awake (jerarquía estática)
- VRHandController usa `laserMaxDistance` para el alcance del láser
