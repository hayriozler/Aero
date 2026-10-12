<!DOCTYPE html>

<html class="light" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>AODB Tactical Hub - Create New Flight</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&amp;family=JetBrains+Mono:wght@500&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    "colors": {
                        "surface-dim": "#dcd9db",
                        "on-tertiary-fixed-variant": "#564427",
                        "primary-fixed": "#d8e3fb",
                        "surface": "#fbf8fa",
                        "on-secondary": "#ffffff",
                        "surface-variant": "#e4e2e3",
                        "surface-bright": "#fbf8fa",
                        "secondary-container": "#d5e3fd",
                        "outline": "#75777d",
                        "on-error-container": "#93000a",
                        "error-container": "#ffdad6",
                        "background": "#fbf8fa",
                        "on-tertiary": "#ffffff",
                        "primary": "#091426",
                        "on-surface": "#1b1b1d",
                        "surface-container-high": "#eae7e9",
                        "error": "#ba1a1a",
                        "surface-container": "#f0edef",
                        "surface-container-low": "#f5f3f4",
                        "inverse-surface": "#303032",
                        "primary-container": "#1e293b",
                        "on-tertiary-container": "#a38c6a",
                        "on-tertiary-fixed": "#271902",
                        "on-secondary-container": "#57657b",
                        "surface-tint": "#545f73",
                        "on-surface-variant": "#45474c",
                        "on-primary-container": "#8590a6",
                        "on-secondary-fixed": "#0d1c2f",
                        "inverse-primary": "#bcc7de",
                        "on-error": "#ffffff",
                        "tertiary-fixed-dim": "#ddc39d",
                        "outline-variant": "#c5c6cd",
                        "on-background": "#1b1b1d",
                        "secondary": "#515f74",
                        "primary-fixed-dim": "#bcc7de",
                        "on-secondary-fixed-variant": "#3a485c",
                        "tertiary-container": "#35260c",
                        "surface-container-highest": "#e4e2e3",
                        "secondary-fixed": "#d5e3fd",
                        "secondary-fixed-dim": "#b9c7e0",
                        "inverse-on-surface": "#f3f0f2",
                        "on-primary-fixed-variant": "#3c475a",
                        "surface-container-lowest": "#ffffff",
                        "on-primary-fixed": "#111c2d",
                        "tertiary": "#1e1200",
                        "tertiary-fixed": "#fadfb8",
                        "on-primary": "#ffffff"
                    },
                    "borderRadius": {
                        "DEFAULT": "0.125rem",
                        "lg": "0.25rem",
                        "xl": "0.5rem",
                        "full": "0.75rem"
                    },
                    "spacing": {
                        "gutter": "8px",
                        "container-padding": "12px",
                        "card-gap": "8px",
                        "unit": "4px",
                        "table-cell-padding": "6px 10px"
                    },
                    "fontFamily": {
                        "label-bold": ["Inter"],
                        "headline-md": ["Inter"],
                        "body-md": ["Inter"],
                        "headline-lg": ["Inter"],
                        "data-mono": ["JetBrains Mono"],
                        "body-sm": ["Inter"]
                    },
                    "fontSize": {
                        "label-bold": ["11px", { "lineHeight": "14px", "letterSpacing": "0.05em", "fontWeight": "700" }],
                        "headline-md": ["16px", { "lineHeight": "24px", "fontWeight": "600" }],
                        "body-md": ["14px", { "lineHeight": "20px", "fontWeight": "400" }],
                        "headline-lg": ["20px", { "lineHeight": "28px", "letterSpacing": "-0.01em", "fontWeight": "600" }],
                        "data-mono": ["13px", { "lineHeight": "18px", "fontWeight": "500" }],
                        "body-sm": ["12px", { "lineHeight": "16px", "fontWeight": "400" }]
                    }
                }
            }
        }
    </script>
<style>
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        .material-symbols-outlined[data-weight="fill"] {
            font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
    </style>
</head>
<body class="bg-background text-on-background font-body-md h-screen overflow-hidden flex flex-col">
<!-- TopNavBar -->
<header class="bg-surface-container-highest text-primary flex justify-between items-center w-full px-container-padding h-12 z-50 border-b border-outline-variant hidden md:flex">
<div class="flex items-center gap-4">
<span class="text-headline-lg font-headline-lg font-black text-primary">AODB Tactical Hub</span>
</div>
<div class="flex items-center gap-4">
<span class="material-symbols-outlined text-on-surface-variant cursor-pointer hover:text-primary transition-colors" data-icon="flight_takeoff">flight_takeoff</span>
<span class="material-symbols-outlined text-on-surface-variant cursor-pointer hover:text-primary transition-colors" data-icon="percent">percent</span>
<span class="material-symbols-outlined text-on-surface-variant cursor-pointer hover:text-primary transition-colors" data-icon="cloud">cloud</span>
<span class="material-symbols-outlined text-on-surface-variant cursor-pointer hover:text-primary transition-colors" data-icon="schedule">schedule</span>
</div>
</header>
<div class="flex flex-1 overflow-hidden">
<!-- SideNavBar -->
<nav class="bg-surface text-primary hidden md:flex flex-col z-40 border-r border-outline-variant w-64 pt-4">
<div class="px-4 pb-6 border-b border-outline-variant mb-4">
<div class="flex items-center gap-3">
<div class="w-10 h-10 rounded-full bg-primary-container text-on-primary-container flex items-center justify-center font-label-bold text-label-bold uppercase">
                        AA
                    </div>
<div>
<div class="text-label-bold font-label-bold text-primary">Control Tower</div>
<div class="text-body-sm font-body-sm text-on-surface-variant">Terminal 1</div>
</div>
</div>
</div>
<div class="flex flex-col gap-1 flex-1 px-2">
<a class="flex items-center gap-3 px-3 py-2 rounded-lg text-on-surface-variant hover:bg-surface-container-high transition-all duration-150 ease-in-out" href="#">
<span class="material-symbols-outlined" data-icon="domain">domain</span>
<span class="text-label-bold font-label-bold">Terminal Views</span>
</a>
<a class="flex items-center gap-3 px-3 py-2 rounded-lg text-on-surface-variant hover:bg-surface-container-high transition-all duration-150 ease-in-out" href="#">
<span class="material-symbols-outlined" data-icon="construction">construction</span>
<span class="text-label-bold font-label-bold">Resource Management</span>
</a>
<a class="flex items-center gap-3 px-3 py-2 rounded-lg text-on-surface-variant hover:bg-surface-container-high transition-all duration-150 ease-in-out" href="#">
<span class="material-symbols-outlined" data-icon="warning">warning</span>
<span class="text-label-bold font-label-bold">Alerts</span>
</a>
<div class="mt-auto pb-4 flex flex-col gap-1">
<a class="flex items-center gap-3 px-3 py-2 rounded-lg text-on-surface-variant hover:bg-surface-container-high transition-all duration-150 ease-in-out" href="#">
<span class="material-symbols-outlined" data-icon="settings">settings</span>
<span class="text-label-bold font-label-bold">Settings</span>
</a>
<a class="flex items-center gap-3 px-3 py-2 rounded-lg text-on-surface-variant hover:bg-surface-container-high transition-all duration-150 ease-in-out" href="#">
<span class="material-symbols-outlined" data-icon="help">help</span>
<span class="text-label-bold font-label-bold">Support</span>
</a>
</div>
</div>
</nav>
<!-- Main Content Canvas -->
<main class="flex-1 overflow-y-auto bg-surface-container-low p-container-padding">
<div class="max-w-5xl mx-auto">
<!-- Page Header -->
<div class="flex items-center justify-between mb-6">
<div class="flex items-center gap-3">
<button class="w-8 h-8 flex items-center justify-center rounded hover:bg-surface-variant transition-colors text-on-surface">
<span class="material-symbols-outlined" data-icon="arrow_back">arrow_back</span>
</button>
<div>
<h1 class="text-headline-lg font-headline-lg text-primary">Create New Flight</h1>
<p class="text-body-sm font-body-sm text-on-surface-variant mt-1">Enter operational details to schedule a new movement.</p>
</div>
</div>
</div>
<!-- Form Card -->
<div class="bg-surface border border-outline-variant rounded-xl shadow-sm overflow-hidden">
<form class="p-6 flex flex-col gap-8">
<!-- Core Identifiers Group -->
<div>
<h2 class="text-headline-md font-headline-md text-primary mb-4 flex items-center gap-2 border-b border-outline-variant pb-2">
<span class="material-symbols-outlined text-primary" data-icon="flight">flight</span>
                                Core Identifiers
                            </h2>
<div class="grid grid-cols-1 md:grid-cols-12 gap-gutter">
<div class="col-span-1 md:col-span-4">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Flight Number *</label>
<input class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-outline" placeholder="e.g., AA123" required="" type="text"/>
</div>
<div class="col-span-1 md:col-span-4">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Carrier *</label>
<select class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-body-md font-body-md text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary appearance-none bg-[url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2224%22%20height%3D%2224%22%20viewBox%3D%220%200%2024%2024%22%20fill%3D%22none%22%20stroke%3D%22%2375777d%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E')] bg-[length:16px] bg-no-repeat bg-[position:right_8px_center] pr-8" required="">
<option disabled="" selected="" value="">Select Carrier</option>
<option value="AA">American Airlines (AA)</option>
<option value="DL">Delta Air Lines (DL)</option>
<option value="UA">United Airlines (UA)</option>
<option value="BA">British Airways (BA)</option>
</select>
</div>
<div class="col-span-1 md:col-span-4">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Movement Type *</label>
<div class="flex h-9 bg-surface-container border border-outline-variant rounded overflow-hidden">
<label class="flex-1 cursor-pointer">
<input checked="" class="peer sr-only" name="flight_type" type="radio" value="arrival"/>
<div class="h-full flex items-center justify-center text-label-bold font-label-bold peer-checked:bg-primary peer-checked:text-on-primary text-on-surface-variant transition-colors">
                                                Arrival
                                            </div>
</label>
<div class="w-[1px] bg-outline-variant"></div>
<label class="flex-1 cursor-pointer">
<input class="peer sr-only" name="flight_type" type="radio" value="departure"/>
<div class="h-full flex items-center justify-center text-label-bold font-label-bold peer-checked:bg-primary peer-checked:text-on-primary text-on-surface-variant transition-colors">
                                                Departure
                                            </div>
</label>
</div>
</div>
</div>
</div>
<!-- Logistics Group -->
<div>
<h2 class="text-headline-md font-headline-md text-primary mb-4 flex items-center gap-2 border-b border-outline-variant pb-2">
<span class="material-symbols-outlined text-primary" data-icon="settings_system_daydream">settings_system_daydream</span>
                                Logistics &amp; Equipment
                            </h2>
<div class="grid grid-cols-1 md:grid-cols-12 gap-gutter">
<div class="col-span-1 md:col-span-3">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Scheduled Date *</label>
<input class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary" required="" type="date"/>
</div>
<div class="col-span-1 md:col-span-3">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Scheduled Time (UTC) *</label>
<input class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary" required="" type="time"/>
</div>
<div class="col-span-1 md:col-span-3">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Aircraft Type *</label>
<input class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-outline" placeholder="e.g., B738" required="" type="text"/>
</div>
<div class="col-span-1 md:col-span-3">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Assigned Gate</label>
<select class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary appearance-none bg-[url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2224%22%20height%3D%2224%22%20viewBox%3D%220%200%2024%2024%22%20fill%3D%22none%22%20stroke%3D%22%2375777d%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E')] bg-[length:16px] bg-no-repeat bg-[position:right_8px_center] pr-8">
<option disabled="" selected="" value="">Select Gate</option>
<option value="A1">Gate A1</option>
<option value="A2">Gate A2</option>
<option value="B1">Gate B1 (Heavy)</option>
<option value="C4">Gate C4</option>
<option value="TBD">Unassigned</option>
</select>
</div>
</div>
</div>
<!-- Routing Group -->
<div>
<h2 class="text-headline-md font-headline-md text-primary mb-4 flex items-center gap-2 border-b border-outline-variant pb-2">
<span class="material-symbols-outlined text-primary" data-icon="route">route</span>
                                Routing
                            </h2>
<div class="grid grid-cols-1 md:grid-cols-12 gap-gutter items-end">
<div class="col-span-1 md:col-span-5">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Origin (IATA) *</label>
<input class="w-full h-9 px-3 bg-surface border border-outline-variant rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-outline uppercase" maxlength="3" placeholder="JFK" required="" type="text"/>
</div>
<div class="hidden md:flex col-span-2 justify-center pb-2 text-outline">
<span class="material-symbols-outlined" data-icon="arrow_right_alt">arrow_right_alt</span>
</div>
<div class="col-span-1 md:col-span-5">
<label class="block text-label-bold font-label-bold text-on-surface-variant mb-1 uppercase">Destination (IATA) *</label>
<input class="w-full h-9 px-3 bg-surface border-outline-variant border rounded text-data-mono font-data-mono text-on-surface focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-outline uppercase" maxlength="3" placeholder="LHR" required="" type="text"/>
</div>
</div>
</div>
<!-- Form Actions -->
<div class="pt-6 mt-4 border-t border-outline-variant flex justify-end gap-3">
<button class="h-9 px-4 rounded border border-outline text-on-surface font-label-bold text-label-bold hover:bg-surface-variant transition-colors uppercase tracking-wider" type="button">
                                Cancel
                            </button>
<button class="h-9 px-6 rounded bg-primary text-on-primary font-label-bold text-label-bold hover:bg-primary-container transition-colors uppercase tracking-wider shadow-sm flex items-center gap-2" type="submit">
<span class="material-symbols-outlined text-[16px]" data-icon="save">save</span>
                                Save Flight
                            </button>
</div>
</form>
</div>
</div>
</main>
</div>
</body></html>