<!DOCTYPE html>

<html class="dark" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>AeroScan BRS - Command Dashboard</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@300;400;500;600;700&amp;family=Inter:wght@300;400;500;600;700&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    "colors": {
                        "inverse-on-surface": "#535557",
                        "inverse-surface": "#f9f9fc",
                        "tertiary-fixed": "#f7e953",
                        "on-primary-fixed-variant": "#5f4200",
                        "outline-variant": "#46484a",
                        "on-primary": "#5f4200",
                        "primary-container": "#feb700",
                        "primary-fixed": "#feb700",
                        "error-container": "#b92902",
                        "surface-bright": "#292c2f",
                        "tertiary-container": "#f7e953",
                        "on-primary-fixed": "#392700",
                        "surface-container-low": "#111416",
                        "on-surface": "#eeeef0",
                        "on-tertiary-fixed": "#484200",
                        "tertiary-dim": "#e8da46",
                        "on-secondary-container": "#fff6f3",
                        "primary-dim": "#ecaa00",
                        "inverse-primary": "#7d5800",
                        "secondary-dim": "#ff7524",
                        "on-secondary": "#3c1300",
                        "tertiary-fixed-dim": "#e8da46",
                        "on-tertiary-container": "#5c5500",
                        "primary": "#ffc965",
                        "outline": "#747578",
                        "on-primary-container": "#533a00",
                        "secondary-fixed": "#ffc5ab",
                        "surface-container": "#171a1c",
                        "surface-variant": "#232629",
                        "on-error-container": "#ffd2c8",
                        "background": "#0c0e10",
                        "on-surface-variant": "#aaabad",
                        "error": "#ff7351",
                        "surface": "#0c0e10",
                        "on-secondary-fixed": "#5c2200",
                        "on-tertiary": "#655d00",
                        "secondary": "#ff7524",
                        "surface-container-highest": "#232629",
                        "secondary-fixed-dim": "#ffb28d",
                        "on-error": "#450900",
                        "surface-dim": "#0c0e10",
                        "on-secondary-fixed-variant": "#8a3700",
                        "on-tertiary-fixed-variant": "#665f00",
                        "error-dim": "#d53d18",
                        "surface-container-lowest": "#000000",
                        "tertiary": "#fff6ab",
                        "primary-fixed-dim": "#ecaa00",
                        "secondary-container": "#a04100",
                        "surface-tint": "#ffc965",
                        "on-background": "#eeeef0",
                        "surface-container-high": "#1d2022"
                    },
                    "borderRadius": {
                        "DEFAULT": "0.125rem",
                        "lg": "0.25rem",
                        "xl": "0.5rem",
                        "full": "0.75rem"
                    },
                    "fontFamily": {
                        "headline": ["Space Grotesk"],
                        "body": ["Inter"],
                        "label": ["Inter"]
                    }
                },
            },
        }
    </script>
<style>
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        body { font-family: 'Inter', sans-serif; }
        h1, h2, h3, .metric { font-family: 'Space Grotesk', sans-serif; }
        .glass { backdrop-filter: blur(20px); background-color: rgba(35, 38, 41, 0.6); }
    </style>
</head>
<body class="bg-background text-on-surface flex h-screen overflow-hidden">
<!-- SideNavBar Shell -->
<aside class="hidden md:flex flex-col h-screen py-4 bg-[#171a1c] w-64 flex-shrink-0 z-50">
<div class="px-6 mb-8">
<div class="flex flex-col">
<span class="font-['Space_Grotesk'] uppercase text-xs font-medium text-[#FFB800]">Live Ops - UTC</span>
<span class="font-['Space_Grotesk'] uppercase text-lg font-bold text-[#eeeef0]">Terminal 5</span>
</div>
</div>
<nav class="flex-1 space-y-1">
<a class="flex items-center px-6 py-3 text-[#FFB800] border-l-4 border-[#FFB800] bg-[#232629] font-['Space_Grotesk'] uppercase text-xs font-medium transition-all duration-200 ease-in-out" href="#">
<span class="material-symbols-outlined mr-4">dashboard</span>
                Dashboard
            </a>
<a class="flex items-center px-6 py-3 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 font-['Space_Grotesk'] uppercase text-xs font-medium transition-all duration-200 ease-in-out" href="#">
<span class="material-symbols-outlined mr-4">flight_takeoff</span>
                Flights
            </a>
<a class="flex items-center px-6 py-3 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 font-['Space_Grotesk'] uppercase text-xs font-medium transition-all duration-200 ease-in-out" href="#">
<span class="material-symbols-outlined mr-4">barcode_scanner</span>
                Scanning
            </a>
<a class="flex items-center px-6 py-3 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 font-['Space_Grotesk'] uppercase text-xs font-medium transition-all duration-200 ease-in-out" href="#">
<span class="material-symbols-outlined mr-4">assessment</span>
                Reports
            </a>
</nav>
<div class="px-6 space-y-4">
<div class="space-y-1">
<a class="flex items-center py-2 text-[#eeeef0]/50 hover:text-[#FFB800] font-['Space_Grotesk'] uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined mr-3 text-sm">settings</span>
                    Settings
                </a>
<a class="flex items-center py-2 text-[#eeeef0]/50 hover:text-[#FFB800] font-['Space_Grotesk'] uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined mr-3 text-sm">help</span>
                    Support
                </a>
</div>
<button class="w-full bg-error-container hover:bg-error text-white py-3 font-bold text-xs uppercase tracking-widest active:scale-95 transition-all">
                EMERGENCY STOP
            </button>
</div>
</aside>
<div class="flex-1 flex flex-col min-w-0 overflow-hidden">
<!-- TopNavBar Shell -->
<header class="flex justify-between items-center w-full px-6 h-16 bg-[#0c0e10] shadow-[0px_2px_8px_rgba(0,0,0,0.5)] z-40 border-none">
<div class="flex items-center space-x-8">
<span class="text-xl font-bold uppercase text-[#eeeef0] font-['Space_Grotesk'] tracking-tight">BRS COMMAND</span>
<nav class="hidden lg:flex space-x-6">
<a class="text-[#FFB800] border-b-2 border-[#FFB800] py-5 transition-colors font-['Space_Grotesk'] tracking-tight" href="#">Real-time Dashboard</a>
<a class="text-[#eeeef0]/60 hover:bg-[#232629] py-5 px-2 transition-colors font-['Space_Grotesk'] tracking-tight" href="#">Analytics</a>
<a class="text-[#eeeef0]/60 hover:bg-[#232629] py-5 px-2 transition-colors font-['Space_Grotesk'] tracking-tight" href="#">Maintenance</a>
</nav>
</div>
<div class="flex items-center space-x-4">
<div class="flex space-x-2">
<button class="p-2 text-[#eeeef0]/60 hover:bg-[#232629] transition-colors active:scale-95">
<span class="material-symbols-outlined">sensors</span>
</button>
<button class="p-2 text-[#eeeef0]/60 hover:bg-[#232629] transition-colors active:scale-95">
<span class="material-symbols-outlined">schedule</span>
</button>
<button class="p-2 text-[#eeeef0]/60 hover:bg-[#232629] transition-colors active:scale-95">
<span class="material-symbols-outlined">account_circle</span>
</button>
</div>
<img alt="Operator Profile" class="w-8 h-8 rounded-full border border-outline-variant" data-alt="professional portrait of an airport terminal operations manager in a dark control room environment" src="https://lh3.googleusercontent.com/aida-public/AB6AXuD68QC0PIVKyrtDiKVpvo-k2XVNG6_yjeBVrGBrnbd5nlORF__b0ayQz5zqRTQsKhFhBlArZoiVJY_l0-8w7sVZQq2tdZPe9G08GYTUBOJYAXQ8qZ7iLAhqfF65tcgBOAk_X4LI3mjcEBWg_hT85-xJ8NxxBjEPNDGVOIyF7bQXDU655ydcsR8Ka0QW7iKfAIVQXiXPqepeexBKHBxdGO3zxOZbEHjpzpAwOY3AAI64wrwC0hhT92FcOU-jTuJSiP2-sViLsdSmnAk"/>
</div>
</header>
<!-- Main Content Canvas -->
<main class="flex-1 overflow-y-auto p-6 space-y-6 bg-surface">
<!-- Telemetry Strip (New Component) -->
<div class="h-1 w-full bg-tertiary-container relative overflow-hidden opacity-80">
<div class="absolute inset-0 bg-gradient-to-r from-transparent via-white/40 to-transparent w-1/4 animate-[shimmer_2s_infinite]" style="animation: shimmer 3s infinite linear;"></div>
</div>
<style>
                @keyframes shimmer { 0% { transform: translateX(-100%); } 100% { transform: translateX(400%); } }
            </style>
<header class="flex justify-between items-end">
<div>
<h1 class="text-4xl font-extrabold tracking-tighter text-on-surface uppercase">Operational Canvas</h1>
<p class="text-on-surface-variant text-sm font-body">Global System Status: <span class="text-primary-container font-bold">NOMINAL</span></p>
</div>
<div class="text-right hidden sm:block">
<span class="block text-xs uppercase text-on-surface-variant font-label tracking-widest">Last Update</span>
<span class="metric text-xl font-medium text-on-surface">14:02:39 UTC</span>
</div>
</header>
<!-- KPI Bento Grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
<!-- Active Flights -->
<div class="bg-surface-container p-6 border-none shadow-lg relative group overflow-hidden">
<div class="absolute top-0 right-0 p-4 text-primary opacity-20">
<span class="material-symbols-outlined text-4xl">flight</span>
</div>
<span class="block text-xs font-label uppercase text-on-surface-variant tracking-widest mb-1">Active Flights</span>
<div class="metric text-5xl font-bold text-on-surface">42</div>
<div class="mt-4 flex items-center text-xs text-on-surface-variant">
<span class="material-symbols-outlined text-sm mr-1 text-primary-container">trending_up</span>
<span class="text-primary-container">+3</span> from previous hour
                    </div>
</div>
<!-- Total Bags Scanned -->
<div class="bg-surface-container-high p-6 border-none shadow-lg relative overflow-hidden">
<span class="block text-xs font-label uppercase text-on-surface-variant tracking-widest mb-1">Total Bags Scanned</span>
<div class="metric text-5xl font-bold text-primary-container">12,842</div>
<div class="mt-4 h-1 w-full bg-surface-container-highest rounded-full overflow-hidden">
<div class="h-full bg-primary-container w-[78%]"></div>
</div>
<div class="mt-2 flex justify-between text-[10px] uppercase font-bold text-on-surface-variant">
<span>Capacity</span>
<span>78%</span>
</div>
</div>
<!-- Reconciliation Errors -->
<div class="bg-surface-container p-6 border-none shadow-lg relative group">
<div class="absolute top-0 right-0 p-4">
<span class="flex h-3 w-3">
<span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-secondary opacity-75"></span>
<span class="relative inline-flex rounded-full h-3 w-3 bg-secondary"></span>
</span>
</div>
<span class="block text-xs font-label uppercase text-on-surface-variant tracking-widest mb-1">Reconciliation Errors</span>
<div class="metric text-5xl font-bold text-secondary">07</div>
<div class="mt-4 flex items-center text-xs text-secondary-dim font-bold">
<span class="material-symbols-outlined text-sm mr-1">warning</span>
                        ACTION REQUIRED
                    </div>
</div>
<!-- System Throughput -->
<div class="bg-surface-container p-6 border-none shadow-lg relative overflow-hidden">
<span class="block text-xs font-label uppercase text-on-surface-variant tracking-widest mb-1">System Throughput</span>
<div class="metric text-5xl font-bold text-on-surface">840<span class="text-lg ml-1 text-on-surface-variant">b/m</span></div>
<div class="mt-4 flex items-center text-xs text-on-surface-variant">
<span class="material-symbols-outlined text-sm mr-1">bolt</span>
                        Peak efficiency reached
                    </div>
</div>
</div>
<!-- Table Section: Reconciliation Progress -->
<div class="bg-surface-container-low rounded-lg overflow-hidden shadow-2xl">
<div class="px-6 py-5 bg-surface-container flex justify-between items-center">
<h2 class="text-xl font-bold uppercase tracking-tight font-headline flex items-center">
<span class="material-symbols-outlined mr-2 text-primary">view_list</span>
                        Flight Reconciliation Progress
                    </h2>
<div class="flex items-center bg-surface-container-highest px-3 py-1.5 rounded text-xs font-label text-on-surface-variant">
<span class="material-symbols-outlined text-sm mr-2">search</span>
                        FILTER FLIGHTS...
                    </div>
</div>
<div class="overflow-x-auto">
<table class="w-full text-left border-collapse">
<thead>
<tr class="bg-surface-container-highest/50 text-on-surface-variant text-[10px] uppercase font-bold tracking-widest">
<th class="px-6 py-4">Flight No</th>
<th class="px-6 py-4">Destination</th>
<th class="px-6 py-4">Departure Time</th>
<th class="px-6 py-4">Reconciled Status</th>
<th class="px-6 py-4 text-right">Actions</th>
</tr>
</thead>
<tbody class="divide-y divide-surface-container-highest/30">
<!-- Row 1: High Priority/Alert -->
<tr class="hover:bg-surface-bright transition-colors group relative bg-surface-bright/20">
<td class="absolute left-0 top-0 bottom-0 w-1 bg-primary"></td>
<td class="px-6 py-4">
<span class="metric font-bold text-primary">BA202</span>
</td>
<td class="px-6 py-4 text-on-surface font-medium">LONDON-LHR</td>
<td class="px-6 py-4 text-on-surface-variant font-mono">14:45 UTC</td>
<td class="px-6 py-4">
<div class="flex items-center space-x-3">
<div class="flex-1 min-w-[120px] h-2 bg-surface-container-highest rounded-full overflow-hidden">
<div class="h-full bg-primary-container w-[92%]"></div>
</div>
<span class="text-xs font-bold text-primary-container">92%</span>
</div>
</td>
<td class="px-6 py-4 text-right">
<button class="text-on-surface-variant hover:text-primary transition-colors">
<span class="material-symbols-outlined">analytics</span>
</button>
</td>
</tr>
<!-- Row 2: Warning/Secondary -->
<tr class="hover:bg-surface-bright transition-colors border-l-4 border-transparent">
<td class="px-6 py-4">
<span class="metric font-bold">EK007</span>
</td>
<td class="px-6 py-4 text-on-surface font-medium">DUBAI-DXB</td>
<td class="px-6 py-4 text-on-surface-variant font-mono">15:10 UTC</td>
<td class="px-6 py-4">
<div class="flex items-center space-x-3">
<div class="flex-1 min-w-[120px] h-2 bg-surface-container-highest rounded-full overflow-hidden">
<div class="h-full bg-secondary w-[45%]"></div>
</div>
<span class="text-xs font-bold text-secondary">45%</span>
</div>
</td>
<td class="px-6 py-4 text-right">
<button class="text-on-surface-variant hover:text-primary transition-colors">
<span class="material-symbols-outlined">analytics</span>
</button>
</td>
</tr>
<!-- Row 3: Standard -->
<tr class="hover:bg-surface-bright transition-colors bg-surface-container-low/50">
<td class="px-6 py-4">
<span class="metric font-bold">AF148</span>
</td>
<td class="px-6 py-4 text-on-surface font-medium">PARIS-CDG</td>
<td class="px-6 py-4 text-on-surface-variant font-mono">15:55 UTC</td>
<td class="px-6 py-4">
<div class="flex items-center space-x-3">
<div class="flex-1 min-w-[120px] h-2 bg-surface-container-highest rounded-full overflow-hidden">
<div class="h-full bg-on-surface-variant w-[12%]"></div>
</div>
<span class="text-xs font-bold text-on-surface-variant">12%</span>
</div>
</td>
<td class="px-6 py-4 text-right">
<button class="text-on-surface-variant hover:text-primary transition-colors">
<span class="material-symbols-outlined">analytics</span>
</button>
</td>
</tr>
<!-- Row 4: Error State -->
<tr class="hover:bg-surface-bright transition-colors border-l-4 border-secondary/40">
<td class="px-6 py-4">
<span class="metric font-bold text-secondary">LH904</span>
</td>
<td class="px-6 py-4 text-on-surface font-medium">FRANKFURT-FRA</td>
<td class="px-6 py-4 text-on-surface-variant font-mono">16:20 UTC</td>
<td class="px-6 py-4">
<div class="flex items-center space-x-3">
<div class="flex-1 min-w-[120px] h-2 bg-surface-container-highest rounded-full overflow-hidden">
<div class="h-full bg-error w-[68%]"></div>
</div>
<div class="flex flex-col">
<span class="text-xs font-bold text-secondary">MISMATCH</span>
<span class="text-[8px] text-error uppercase">6 Unscanned</span>
</div>
</div>
</td>
<td class="px-6 py-4 text-right">
<button class="text-secondary hover:text-secondary-fixed transition-colors">
<span class="material-symbols-outlined">error_med</span>
</button>
</td>
</tr>
</tbody>
</table>
</div>
<div class="px-6 py-3 bg-surface-container-highest/20 flex justify-end">
<button class="text-[10px] uppercase font-bold tracking-widest text-primary-container flex items-center hover:underline">
                        View all 42 active flights
                        <span class="material-symbols-outlined text-sm ml-1">arrow_forward</span>
</button>
</div>
</div>
<!-- Bottom Layout: Scanning Log & Map Asymmetric Grid -->
<div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
<!-- Live Scanning Feed -->
<div class="lg:col-span-2 bg-surface-container p-6 overflow-hidden">
<h3 class="text-sm font-bold uppercase tracking-widest text-on-surface-variant mb-4 flex items-center">
<span class="material-symbols-outlined mr-2 text-xs">radio_button_checked</span>
                        Live Telemetry Feed
                    </h3>
<div class="space-y-3 font-mono text-[11px]">
<div class="flex items-center text-primary-container bg-primary-container/5 p-2 border-l-2 border-primary-container">
<span class="w-20 shrink-0">14:02:38</span>
<span class="w-32 shrink-0 font-bold">[SCAN_OK]</span>
<span class="flex-1 truncate">TAG: 0012849201 | FLT: BA202 | DEST: LHR | GATE: B42</span>
</div>
<div class="flex items-center text-on-surface-variant p-2">
<span class="w-20 shrink-0">14:02:35</span>
<span class="w-32 shrink-0 font-bold text-on-surface">[ROUT_MSG]</span>
<span class="flex-1 truncate">Baggage segment T5-A4 active. Throughput optimal.</span>
</div>
<div class="flex items-center text-secondary bg-secondary/5 p-2 border-l-2 border-secondary">
<span class="w-20 shrink-0">14:02:31</span>
<span class="w-32 shrink-0 font-bold">[RECON_ERR]</span>
<span class="flex-1 truncate">MISMATCH: TAG 0012849188 not found in manifest LH904</span>
</div>
<div class="flex items-center text-on-surface-variant p-2">
<span class="w-20 shrink-0">14:02:28</span>
<span class="w-32 shrink-0 font-bold text-on-surface">[SCAN_OK]</span>
<span class="flex-1 truncate">TAG: 0012849187 | FLT: EK007 | DEST: DXB | GATE: C11</span>
</div>
</div>
</div>
<!-- Secondary Visual Panel -->
<div class="bg-surface-container overflow-hidden flex flex-col">
<div class="h-48 bg-surface-container-highest relative">
<img alt="Network Map" class="w-full h-full object-cover opacity-60 grayscale hover:grayscale-0 transition-all duration-700" data-alt="simplified top-down 3D schematic map of airport terminal luggage conveyor belts and sorting nodes" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAl8uH7_5H9NTQXBAPU4k18XinxkZ_lyCnELXTnNGm2vQkUBPxM6uASoCHvh53GO38aKO7-u_wzILl8LArbpmtsaZjY3v4itrxQxpnMHl2ayifXJUT6uQRyETthZus_7pcTWy1RARGDZJxUFcapk8EeWrAA1PZcvoBswyDJM5Zs1t1q6z5KYuxwHQJiAbq37aA74N9cc4oVrvAlmJR4fOc91yU9fmeiuwReg-bvAv4a5hBwAngXHDD0sZ7lyiqHII2LcePD6cmfl8E"/>
<div class="absolute inset-0 bg-gradient-to-t from-surface-container to-transparent"></div>
<div class="absolute bottom-4 left-4">
<span class="block text-[10px] uppercase font-bold text-primary-container">Network Node</span>
<span class="metric text-lg font-bold">SEGMENT_T5_ALPHA</span>
</div>
</div>
<div class="p-6 pt-0 space-y-4">
<div class="flex justify-between items-center text-xs">
<span class="text-on-surface-variant uppercase">Node Health</span>
<span class="text-primary-container font-bold">99.2%</span>
</div>
<div class="w-full h-1 bg-surface-container-highest rounded-full">
<div class="h-full bg-primary-container w-[99.2%]"></div>
</div>
<p class="text-[10px] text-on-surface-variant leading-relaxed italic">
                            System monitoring active on all 14 sorting junctions. No mechanical friction alerts detected in the last 60 minutes.
                        </p>
</div>
</div>
</div>
</main>
</div>
<!-- Responsive Bottom Nav for Mobile -->
<nav class="md:hidden fixed bottom-0 left-0 right-0 h-16 bg-[#0c0e10] flex justify-around items-center px-6 z-50 shadow-[0px_-2px_8px_rgba(0,0,0,0.5)]">
<a class="flex flex-col items-center text-[#FFB800]" href="#">
<span class="material-symbols-outlined">dashboard</span>
<span class="text-[10px] uppercase font-bold mt-1">Dash</span>
</a>
<a class="flex flex-col items-center text-[#eeeef0]/60" href="#">
<span class="material-symbols-outlined">flight_takeoff</span>
<span class="text-[10px] uppercase font-bold mt-1">Flights</span>
</a>
<a class="flex flex-col items-center text-[#eeeef0]/60" href="#">
<span class="material-symbols-outlined">barcode_scanner</span>
<span class="text-[10px] uppercase font-bold mt-1">Scan</span>
</a>
<a class="flex flex-col items-center text-[#eeeef0]/60" href="#">
<span class="material-symbols-outlined">assessment</span>
<span class="text-[10px] uppercase font-bold mt-1">Logs</span>
</a>
</nav>
</body></html>