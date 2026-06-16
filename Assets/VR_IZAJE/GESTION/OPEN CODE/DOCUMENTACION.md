# VR IZAJE — Documentación del Proyecto

Aplicación VR para capacitar al personal en procedimientos de izaje industrial.  
18 pasos educativos guiados por asistente robot, con maquetas interactivas, selección de equipos, y cinemática final de accidente.

**Motor:** Unity 2022.3+ · **Target:** Oculus Quest/Go · **Pipeline:** Built-in  
**Paquetes:** XR Interaction Toolkit 3.x, XR Core Utils, TextMeshPro  
**Repo:** https://github.com/OrigenDevs/VR_IZAGE.git

---

## Estructura del proyecto

```
Assets/VR_IZAJE/
├── CODIGO/
│   ├── Sistema/          StepManager, StepData, DialogSystemVR
│   ├── Animacion/        NPCPatrol, LipSyncController, OscilacionAutomatica, RotacionAutomatica
│   ├── UI/               UI_ConfirmPopup
│   └── VR/               DesktopInputController, VRHandController, SimpleGrab, Button3D
├── Editor/               SimpleVRSetup (Ctrl+Shift+V)
├── ESCENAS/
│   ├── HOME.unity        Menú principal
│   ├── LOBBI.unity       Escena principal del curso
│   └── TRABAJO DE CAMPO IZAJE.unity  Escena de campo (fase 4)
├── 3D/
│   ├── Modelos/          FBX base + específicos
│   ├── ANIMACIONES/      Animaciones de grúa (accidente, éxito)
│   ├── HDRI/             Cielos y materiales de ambiente
│   └── MATERIALES/       Materiales básicos
├── PREFABS/
│   ├── 01_ESTRUCTURALES/ Paredes, fondos, cercas, rocas, montañas
│   ├── 02_MEDIANOS/      Grúas, vehículos, cajas de carga
│   ├── 03_DECORATIVOS/   Lámparas, sillas, escaleras, conos, señales
│   ├── 04_INTERACTIVOS/  EPP, radio, anemómetro, celular, documentos
│   └── PERSONAJES/
│       ├── ROBOT/        Robot asistente (modelo, animaciones, sprites)
│       └── NPC/          Trabajadores (controllers, prefabs)
├── SONIDO/
│   ├── sonido_accidente.mp3
│   └── sonido_carga_exitosa.mp3
└── GESTION/
    ├── DAVID/            Documentos de David (contenido, progreso, guías)
    ├── JUAN/             Espacio de trabajo de Juan
    └── OPEN CODE/        Documentación del proyecto + REPORTE.txt
```

---

## Sistema de 18 Pasos

### StepManager (director central)
- Array público `StepData[]` cargado en Start()
- `LoadStep(index)`: activa/desactiva objetos, inicia diálogo, espera condición
- `GoToStep(index)`: salta a cualquier paso
- `NotifyComplete()`: para finalización externa

### StepData (ScriptableObject por paso)
| Campo | Tipo | Uso |
|-------|------|-----|
| stepNumber | int | Orden del paso |
| voiceClip | AudioClip | Voz del robot |
| dialogText | string | Texto sincronizado |
| completionType | enum | Auto, ButtonConfirm, Timer, GrabObject, Measurement, Sequence |
| objectsToActivate[] | GameObject[] | Se encienden al entrar |
| objectsToDeactivate[] | GameObject[] | Se apagan al entrar |
| completionTimer | float | Segundos si es Timer |
| completionMessage | string | Texto del popup |
| completionTarget | GameObject | Objeto a interactuar |
| timeAdjustment | float (-5 a 5) | Ajuste fino voz/texto |
| extraSystem | enum | None, VirtualShelf, Diorama, WindMeter, HookInspection, LoadTest |

### Flujo por paso
```
Activar objetos → Voz + texto sincronizado → Animar boca (LipSync) → Esperar acción → Avanzar
```

---

## Los 18 Pasos

### FASE 00 — Introducción
1. Robot se presenta, explica izaje, presenta Rigger y Operador

### FASE 1 — Certificaciones (modelos 3D)
2. Certificación del Rigger
3. Certificación de la grúa móvil
4. Mantenciones al día
5. Aprobación del plan de izaje

### FASE 2 — Maqueta interactiva
6. Estabilización de la grúa
7. Centro de gravedad + eliminar obstáculos
8. Área segregada (arrastrar conos)
9. Informar al personal vía celular

### FASE 3 — Mesa de inspección
10. Checklist de EPP
11. Selección de accesorios de izaje
12. Elemento seguro vs dañado
13. Sistema de comunicación
14. Notificar protocolo vía celular

### FASE 4 — Campo
15. Verificación climática (anemómetro)
16. Disposición final de la carga
17. Confirmación con operador vía radio

### FASE FINAL
18. Cinemática de accidente + reflexión

---

## Scripts del sistema

### StepManager.cs
`CODIGO/Sistema/StepManager.cs`
- `LoadStep(int)`, `GoToStep(int)`, `NotifyComplete()`
- 4 referencias públicas: dialogSystem, lipSync, confirmPopup, currentStepIndex
- Escucha callback del diálogo y decide avance según completionType

### StepData.cs
`CODIGO/Sistema/StepData.cs`
- Menú: Assets/Create/VR Izaje/Paso
- Almacenar idealmente en `Assets/VR_IZAJE/DATA/` (pendiente de crear)

### DialogSystemVR.cs
`CODIGO/Sistema/DialogSystemVR.cs`
- TextMeshProUGUI + AudioSource
- Calcula tiempo por carácter = duraciónAudio / largoTexto
- `Play(clip, text, timeAdjust, callback)`, `Stop()`

### NPCPatrol.cs
`CODIGO/Animacion/NPCPatrol.cs`
- Patrulla A→B con idle aleatorio (minIdleTime, maxIdleTime)
- `animator.SetBool("Caminar", true/false)`
- Rotación suave con `Quaternion.RotateTowards`

### LipSyncController.cs
`CODIGO/Animacion/LipSyncController.cs`
- Sprites de boca según espectro de audio (GetSpectrumData)
- Silencio → sprite[0]; con voz → ciclo de sprites

### DesktopInputController.cs
`CODIGO/VR/DesktopInputController.cs`
- Tecla P alterna VR/Escritorio
- Flechas rotan cámara, mouse controla mano, click = gatillo
- Inyecta valores en VRHandController.desktopOverride/Position/Rotation/Trigger

### VRHandController.cs
`CODIGO/VR/VRHandController.cs`
- Láser (LineRenderer) con mirilla (reticle esférica) en punto de impacto
- Colores: default/hover/select/trigger/noDevice
- Si desktopOverride=true, ignora tracking real
- Detecta SimpleGrab y Button3D en objetos apuntados, maneja agarre y botones
- GrabAttachPoint: Transform hijo de la mano donde se posicionan objetos agarrados

### SimpleGrab.cs
`CODIGO/VR/SimpleGrab.cs`
- Reemplaza XRGrabInteractable con sistema propio más simple
- Al agarrar: objeto vuela hacia grabAttachPoint de la mano (velocidad configurable)
- Al soltar: objeto vuelve a su posición original
- FaceCamera: el objeto mira hacia la cámara mientras está agarrado
- Maneja Rigidbody (pone kinematic durante agarre)

### Button3D.cs
`CODIGO/VR/Button3D.cs`
- Botón 3D interactivo, funciona con láser/mirilla de la mano
- Hover: escala 1.2 (configurable), Press: escala 0.8, Release: vuelve a 1
- Reproduce AudioSource al soltar el gatillo
- No requiere Canvas ni EventSystem

### UI_ConfirmPopup.cs
`CODIGO/UI/UI_ConfirmPopup.cs`
- Popup con botón "Entendí", callback al confirmar

### SimpleVRSetup.cs (Editor)
`Editor/SimpleVRSetup.cs`
- Menú: VR Tools/Setup Rapido (Ctrl+Shift+V)
- Configura XR Origin con controllers, colliders, hand models

---

## Modo Escritorio

**Setup:** GameObject InputManager → DesktopInputController → referencias a Camera y Left Hand
**Toggle:** Tecla P durante Play
**Controles:** Flechas (cámara), Mouse (mano), Click (gatillo)
**Requiere:** 0 dispositivos VR conectados

`DesktopInputController` activa `desktopOverride=true` en `VRHandController`, que ignora el tracking real y usa valores del mouse.

---

## Prefabs disponibles

### Personajes
- Robot asistente: `PREFABS/PERSONAJES/ROBOT/`
- NPC trabajador: `PREFABS/PERSONAJES/NPC/Trabajador_01.prefab`

### Interactivos
- EPP: casco, chaleco, guantes, botas, gafas, audífonos
- Herramientas: radio, celular, anemómetro, documentos, plataforma robot
- Grúa principal con animaciones de izaje

### Escenografía
- Estructurales: paredes, fondos, cercas, torres, rocas, montañas
- Medianos: grúas, vehículos, cajas, sets de construcción
- Decorativos: lámparas, sillas, escaleras, cintas, conos, señales

---

## Estado del proyecto (16 Junio 2026)

### Completado 100%
Investigación, modelos 3D, UI/Canvas, audios (32 pistas), SFX, biblioteca de prefabs, escenarios, animaciones robot/grúa, scripts del sistema, modo escritorio.

### Implementado en código
- [x] Láser con mirilla visual (reticle esférica con offset anti z-fighting)
- [x] SimpleGrab: agarre simplificado con velocidad configurable y FaceCamera
- [x] Button3D: botones 3D con hover, press y sonido al soltar
- [x] OscilacionAutomatica (posición local en vez de global)
- [x] Manejo de Rigidbody durante agarre (kinematic)
- [x] Dispositivo perdido: release automático de objetos agarrados

### Pendiente — Montaje en Unity
- [ ] Importar/configurar animaciones del robot en Animator Controller
- [ ] Prefabricar NPCs con NPCPatrol y animaciones walk/idle
- [ ] Importar audios y asignar a DialogSystemVR
- [ ] Colocar modelos 3D y botones en LOBBI y Campo
- [ ] Crear 18 StepData ScriptableObjects
- [ ] Configurar StepManager con los 18 pasos
- [ ] Conectar Button3D al sistema de pasos
- [ ] Configurar cinemática de accidente
- [ ] Probar flujo completo en modo escritorio
- [ ] Corrección de bugs
- [ ] Build APK

### Timeline
Días productivos: 1-3 Jun, 8-11, 16 Jun · Meta 19 Jun · 3 días hábiles restantes

---

## Para comenzar

1. Abrir proyecto en Unity 2022.3+
2. Ir a `ESCENAS/LOBBI.unity`
3. Presionar P durante Play para modo escritorio
4. Leer `CODIGO/Sistema/StepManager.cs`, `StepData.cs`, `VR/DesktopInputController.cs`
5. Crear 18 StepData: Assets/Create/VR Izaje/Paso
6. Asignar pasos al StepManager en DirectorFases
7. Probar flujo en modo escritorio

### Notas
- Diálogos completos en `GESTION/DAVID/Contenido/Dialogos.txt`
- Audios: 32 archivos .m4a numerados (00_menu, 00_intro, 0101...1802)
- No confundir número de paso con número de audio (ej: paso 1 = 0101+0102)
- NPCs usan NPCPatrol.cs → configurar puntos A/B
- Robot usa LipSyncController → asignar sprites de boca
- Grúa usa `Grua_principal.controller` para animaciones
- Hacer commits pequeños con mensaje `[Área] Descripción`
- Actualizar `GESTION/OPEN CODE/REPORTE.txt` al empezar y terminar cada día
