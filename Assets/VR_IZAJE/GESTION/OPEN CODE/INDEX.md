# OPEN CODE — Guía de inicio

Punto de entrada para la instancia de Open Code (modelo Big Pickle).  
Todo el proyecto vive dentro de `Assets/VR_IZAJE/` — no uses rutas fuera de aquí.

## Documentación principal
→ Leer `GESTION/OPEN CODE/DOCUMENTACION.md` (estructura, scripts, pasos, estado)

## Archivos críticos

### Scripts del sistema
- `Assets/VR_IZAJE/CODIGO/Sistema/StepManager.cs`
- `Assets/VR_IZAJE/CODIGO/Sistema/StepData.cs`
- `Assets/VR_IZAJE/CODIGO/Sistema/DialogSystemVR.cs`
- `Assets/VR_IZAJE/CODIGO/Animacion/NPCPatrol.cs`
- `Assets/VR_IZAJE/CODIGO/Animacion/LipSyncController.cs`
- `Assets/VR_IZAJE/CODIGO/UI/UI_ConfirmPopup.cs`
- `Assets/VR_IZAJE/CODIGO/VR/DesktopInputController.cs`
- `Assets/VR_IZAJE/CODIGO/VR/VRHandController.cs`
- `Assets/VR_IZAJE/CODIGO/VR/SimpleGrab.cs`
- `Assets/VR_IZAJE/CODIGO/VR/Button3D.cs`
- `Assets/VR_IZAJE/CODIGO/Animacion/OscilacionAutomatica.cs`
- `Assets/VR_IZAJE/CODIGO/Animacion/RotacionAutomatica.cs`
- `Assets/VR_IZAJE/Editor/SimpleVRSetup.cs`

### Documentación de gestión
- `GESTION/DAVID/Contenido/Dialogos.txt` — Todos los diálogos del curso
- `GESTION/DAVID/Guías de programación/ModoEscritorio.html`
- `GESTION/DAVID/Guías de programación/Estructura.html`
- `GESTION/DAVID/Contenido/Notas.html` — Notas educativas
- `GESTION/DAVID/Progreso/Dashboard.html` — Dashboard visual
- `GESTION/DAVID/Progreso/Diario.txt` — Historial de trabajo

### Escenas Unity
- `Assets/VR_IZAJE/ESCENAS/LOBBI.unity` — Escena principal del curso
- `Assets/VR_IZAJE/ESCENAS/HOME.unity` — Menú principal
- `Assets/VR_IZAJE/ESCENAS/TRABAJO DE CAMPO IZAJE.unity` — Escena de campo

---

## Flujo de trabajo (David + Juan)

### Roles
- **David:** Dueño del proyecto, toma decisiones, define contenido, revisa avances
- **Juan:** Desarrollador Unity, implementa código, monta escenas, prueba

### Ciclo diario
1. **Juan** abre el proyecto, revisa el REPORTE.txt y la documentación
2. **Juan** reporta en REPORTE.txt lo que va a hacer hoy
3. **Juan** trabaja, hace commits con mensajes claros
4. **Juan** actualiza REPORTE.txt con lo que hizo, problemas, próximos pasos
5. **David** revisa REPORTE.txt y el Dashboard, da feedback

### REPORTE.txt
Ubicación: `GESTION/OPEN CODE/REPORTE.txt`
Formato libre, pero incluir siempre:
```
## DD/MM/AAAA — [Nombre]

### Qué hice hoy
- ...

### Problemas / dudas
- ...

### Próximos pasos
- ...
```

### Dashboard
El archivo `GESTION/DAVID/Progreso/Dashboard.html` se actualiza manualmente:
- Tachar tareas completadas (`<span class="cb">✓</span>`)
- Ajustar fechas, barras de progreso, timeline
- David lo mantiene, Juan puede sugerir cambios

### Commits
- Hacer commits pequeños por tarea
- Mensaje: `[Área] Descripción breve`
  - Ej: `[StepManager] Fix avanzar tras diálogo`
  - Ej: `[Escena] Colocar prefabs fase 1`
- Hacer push al final del día
- NO commitear archivos .meta de assets no modificados intencionalmente

---

## Prioridades de trabajo

1. Leer todos los scripts del sistema
2. Abrir LOBBI.unity y explorar jerarquía
3. Crear los 18 StepData con sus audios/textos
4. Asignar referencias en StepManager, DialogSystemVR, LipSyncController
5. Colocar modelos 3D y prefabs en escena
6. Probar flujo en modo escritorio (tecla P)
7. Configurar NPCs con NPCPatrol
8. Configurar animaciones del robot (Animator Controller)
9. Configurar cinemática de accidente
10. Pruebas finales y build APK

## Comandos clave
- **P** durante Play → Alterna VR / Escritorio
- **Flechas** → Rotan cámara (modo escritorio)
- **Mouse + click** → Mano/láser (modo escritorio)
- **Ctrl+Shift+V** → Setup rápido VR (editor)
- **Assets/Create/VR Izaje/Paso** → Crear StepData

## Nota importante
El modo escritorio funciona SIN Oculus. Úsalo para todo antes de probar en VR.
