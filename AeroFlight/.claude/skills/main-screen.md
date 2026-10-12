<!DOCTYPE html>

<html class="h-full" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>AODB Tactical Hub - Spatial Awareness</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&amp;family=JetBrains+Mono:wght@500&amp;display=swap" rel="stylesheet"/>
<script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    "colors": {
                        "primary-container": "#1e293b",
                        "surface-container-low": "#f5f3f4",
                        "inverse-surface": "#303032",
                        "surface-container": "#f0edef",
                        "on-secondary-container": "#57657b",
                        "on-tertiary-fixed": "#271902",
                        "on-tertiary-container": "#a38c6a",
                        "on-secondary-fixed": "#0d1c2f",
                        "inverse-primary": "#bcc7de",
                        "on-primary-container": "#8590a6",
                        "on-surface-variant": "#45474c",
                        "surface-tint": "#545f73",
                        "outline-variant": "#c5c6cd",
                        "tertiary-fixed-dim": "#ddc39d",
                        "on-error": "#ffffff",
                        "tertiary-container": "#35260c",
                        "surface-container-highest": "#e4e2e3",
                        "secondary-fixed": "#d5e3fd",
                        "primary-fixed-dim": "#bcc7de",
                        "on-secondary-fixed-variant": "#3a485c",
                        "secondary": "#515f74",
                        "on-background": "#1b1b1d",
                        "tertiary": "#1e1200",
                        "surface-container-lowest": "#ffffff",
                        "on-primary-fixed": "#111c2d",
                        "on-primary-fixed-variant": "#3c475a",
                        "secondary-fixed-dim": "#b9c7e0",
                        "inverse-on-surface": "#f3f0f2",
                        "tertiary-fixed": "#fadfb8",
                        "on-primary": "#ffffff",
                        "primary-fixed": "#d8e3fb",
                        "surface-dim": "#dcd9db",
                        "on-tertiary-fixed-variant": "#564427",
                        "surface-bright": "#fbf8fa",
                        "surface-variant": "#e4e2e3",
                        "surface": "#fbf8fa",
                        "on-secondary": "#ffffff",
                        "outline": "#75777d",
                        "secondary-container": "#d5e3fd",
                        "on-error-container": "#93000a",
                        "error-container": "#ffdad6",
                        "background": "#fbf8fa",
                        "on-tertiary": "#ffffff",
                        "surface-container-high": "#eae7e9",
                        "error": "#ba1a1a",
                        "primary": "#091426",
                        "on-surface": "#1b1b1d"
                    },
                    "borderRadius": {
                        "DEFAULT": "0.5rem",
                        "md": "0.75rem",
                        "lg": "1rem",
                        "xl": "1.5rem",
                        "2xl": "2rem",
                        "full": "9999px"
                    },
                    "spacing": {
                        "container-padding": "16px",
                        "unit": "4px",
                        "table-cell-padding": "8px 12px",
                        "gutter": "16px",
                        "card-gap": "16px"
                    },
                    "fontFamily": {
                        "body-md": ["Inter"],
                        "headline-md": ["Inter"],
                        "label-bold": ["Inter"],
                        "headline-lg": ["Inter"],
                        "data-mono": ["JetBrains Mono"],
                        "body-sm": ["Inter"]
                    },
                    "fontSize": {
                        "label-bold": ["11px", { "lineHeight": "14px", "letterSpacing": "0.05em", "fontWeight": "700" }],
                        "data-mono": ["13px", { "lineHeight": "18px", "fontWeight": "500" }],
                        "body-md": ["14px", { "lineHeight": "20px", "fontWeight": "400" }],
                        "headline-md": ["16px", { "lineHeight": "24px", "fontWeight": "600" }],
                        "headline-lg": ["20px", { "lineHeight": "28px", "letterSpacing": "-0.01em", "fontWeight": "600" }],
                        "body-sm": ["12px", { "lineHeight": "16px", "fontWeight": "400" }]
                    }
                }
            }
        }
    </script>
</head>
<body class="bg-surface-container-low text-on-surface h-full flex flex-col overflow-hidden font-body-md text-body-md">
<!-- Top Navigation Bar (Simplified) -->
<header class="bg-surface-container-lowest shadow-sm flex justify-between items-center w-full px-container-padding h-14 z-50 shrink-0">
<div class="flex items-center gap-6">
<div class="flex items-center gap-2">
<span class="material-symbols-outlined text-primary text-[28px]">radar</span>
<h1 class="text-headline-md font-headline-md text-primary tracking-tight">Tactical Hub <span class="text-on-surface-variant font-normal">| Spatial View</span></h1>
</div>
<div class="hidden md:flex items-center bg-surface-container rounded-full px-4 py-1.5 h-9">
<span class="material-symbols-outlined text-[18px] text-on-surface-variant mr-2">search</span>
<input class="bg-transparent border-none text-body-sm font-body-sm text-on-surface focus:ring-0 p-0 w-64 placeholder-on-surface-variant" placeholder="Search flights, gates, resources..." type="text"/>
</div>
</div>
<div class="flex items-center gap-3">
<button class="p-2 rounded-full hover:bg-surface-container transition-colors text-on-surface-variant flex items-center justify-center">
<span class="material-symbols-outlined text-[20px]">notifications</span>
</button>
<div class="w-9 h-9 rounded-full bg-primary text-on-primary flex items-center justify-center text-label-bold font-label-bold shadow-sm cursor-pointer">
                AD
            </div>
</div>
</header>
<div class="flex flex-1 overflow-hidden h-full">
<!-- Thin Icon-Only Sidebar -->
<aside class="hidden md:flex flex-col z-40 bg-surface-container-lowest shadow-sm w-16 py-4 shrink-0 items-center justify-between">
<nav class="flex flex-col gap-4 w-full px-2">
<a class="w-full aspect-square rounded-xl bg-primary text-on-primary flex flex-col items-center justify-center shadow-md" href="#">
<span class="material-symbols-outlined text-[24px]">map</span>
</a>
<a class="w-full aspect-square rounded-xl text-on-surface-variant hover:bg-surface-container transition-colors flex flex-col items-center justify-center relative" href="#">
<span class="material-symbols-outlined text-[24px]">view_timeline</span>
</a>
<a class="w-full aspect-square rounded-xl text-on-surface-variant hover:bg-surface-container transition-colors flex flex-col items-center justify-center" href="#">
<span class="material-symbols-outlined text-[24px]">construction</span>
</a>
<a class="w-full aspect-square rounded-xl text-on-surface-variant hover:bg-surface-container transition-colors flex flex-col items-center justify-center relative" href="#">
<span class="material-symbols-outlined text-[24px]">warning</span>
<span class="absolute top-1 right-1 w-2.5 h-2.5 bg-error rounded-full border-2 border-surface-container-lowest"></span>
</a>
</nav>
<div class="flex flex-col gap-4 w-full px-2">
<a class="w-full aspect-square rounded-xl text-on-surface-variant hover:bg-surface-container transition-colors flex flex-col items-center justify-center" href="#">
<span class="material-symbols-outlined text-[24px]">settings</span>
</a>
</div>
</aside>
<!-- Main Content Area (Dual View) -->
<main class="flex-1 flex flex-col h-full overflow-hidden p-container-padding gap-gutter">
<!-- Top Half: Map View -->
<div class="flex-1 bg-surface-container-lowest rounded-2xl shadow-sm border border-outline-variant/30 flex flex-col overflow-hidden relative min-h-[40%]">
<div class="absolute top-4 left-4 z-10">
<h2 class="text-headline-md font-headline-md text-primary">Terminal 1 Concourse Map</h2>
<p class="text-body-sm text-on-surface-variant">Live Spatial Overview</p>
</div>
<div class="absolute top-4 right-4 z-10 flex gap-2">
<span class="px-2 py-1 bg-surface-container rounded-lg text-[10px] font-bold text-on-surface flex items-center gap-1"><span class="w-2 h-2 rounded-full bg-[#10B981]"></span> On Time</span>
<span class="px-2 py-1 bg-surface-container rounded-lg text-[10px] font-bold text-on-surface flex items-center gap-1"><span class="w-2 h-2 rounded-full bg-[#F59E0B]"></span> Delayed</span>
<span class="px-2 py-1 bg-surface-container rounded-lg text-[10px] font-bold text-on-surface flex items-center gap-1"><span class="w-2 h-2 rounded-full bg-error"></span> Conflict</span>
</div>
<!-- Schematic Map Area -->
<div class="flex-1 flex items-center justify-center p-8 bg-[#F8FAFC]">
<div class="relative w-full max-w-4xl h-48 flex items-center justify-center">
<!-- Main Concourse Spine -->
<div class="absolute w-full h-12 bg-surface-container-high rounded-full shadow-inner border border-outline-variant/50"></div>
<!-- Gates Top -->
<div class="absolute top-0 w-full flex justify-around px-12 h-1/2">
<!-- G10 -->
<div class="relative flex flex-col items-center -mt-8 cursor-pointer group">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-t border-outline-variant/50 rounded-t-sm"></div>
<div class="absolute -top-10 flex flex-col items-center gap-1">
<div class="w-12 h-12 rounded-full bg-surface-container-lowest shadow-md border-2 border-[#10B981] flex items-center justify-center text-[#10B981] group-hover:scale-110 transition-transform ring-4 ring-[#10B981]/20">
<span class="material-symbols-outlined text-[24px]">flight</span>
</div>
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-outline-variant/30 text-on-surface">G10</span>
</div>
</div>
<!-- G12 -->
<div class="relative flex flex-col items-center -mt-8 cursor-pointer group">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-t border-outline-variant/50 rounded-t-sm"></div>
<div class="absolute -top-10 flex flex-col items-center gap-1">
<div class="w-12 h-12 rounded-full bg-surface-container-lowest shadow-md border-2 border-[#10B981] flex items-center justify-center text-[#10B981] group-hover:scale-110 transition-transform">
<span class="material-symbols-outlined text-[24px]">flight</span>
</div>
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-outline-variant/30 text-on-surface">G12</span>
</div>
</div>
<!-- G14 -->
<div class="relative flex flex-col items-center -mt-8 cursor-pointer group opacity-60">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-t border-outline-variant/50 rounded-t-sm"></div>
<div class="absolute -top-10 flex flex-col items-center gap-1">
<div class="w-12 h-12 rounded-full bg-surface-container-lowest shadow-sm border-2 border-outline-variant border-dashed flex items-center justify-center text-outline-variant group-hover:scale-110 transition-transform">
<span class="material-symbols-outlined text-[20px]">add</span>
</div>
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-outline-variant/30 text-on-surface-variant">G14</span>
</div>
</div>
</div>
<!-- Gates Bottom -->
<div class="absolute bottom-0 w-full flex justify-around px-32 h-1/2">
<!-- G11 -->
<div class="relative flex flex-col items-center mt-8 cursor-pointer group">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-b border-outline-variant/50 rounded-b-sm"></div>
<div class="absolute -bottom-10 flex flex-col items-center gap-1">
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-outline-variant/30 text-on-surface">G11</span>
<div class="w-12 h-12 rounded-full bg-surface-container-lowest shadow-md border-2 border-[#F59E0B] flex items-center justify-center text-[#F59E0B] group-hover:scale-110 transition-transform">
<span class="material-symbols-outlined text-[24px]">flight</span>
</div>
</div>
</div>
<!-- G13 -->
<div class="relative flex flex-col items-center mt-8 cursor-pointer group">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-b border-outline-variant/50 rounded-b-sm"></div>
<div class="absolute -bottom-10 flex flex-col items-center gap-1">
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-error/50 text-error">G13</span>
<div class="w-12 h-12 rounded-full bg-error-container shadow-md border-2 border-error flex items-center justify-center text-error group-hover:scale-110 transition-transform animate-pulse">
<span class="material-symbols-outlined text-[24px]">warning</span>
</div>
</div>
</div>
<!-- G15 -->
<div class="relative flex flex-col items-center mt-8 cursor-pointer group opacity-60">
<div class="w-4 h-8 bg-surface-container-high border-l border-r border-b border-outline-variant/50 rounded-b-sm"></div>
<div class="absolute -bottom-10 flex flex-col items-center gap-1">
<span class="text-label-bold bg-surface-container-lowest px-2 py-0.5 rounded-md shadow-sm border border-outline-variant/30 text-on-surface-variant">G15</span>
<div class="w-12 h-12 rounded-full bg-surface-container-lowest shadow-sm border-2 border-outline-variant border-dashed flex items-center justify-center text-outline-variant group-hover:scale-110 transition-transform">
<span class="material-symbols-outlined text-[20px]">add</span>
</div>
</div>
</div>
</div>
</div>
</div>
</div>
<!-- Bottom Half: Gantt Timeline -->
<div class="flex-1 bg-surface-container-lowest rounded-2xl shadow-sm border border-outline-variant/30 flex flex-col overflow-hidden min-h-[35%]">
<div class="h-[40px] bg-surface-container/50 border-b border-outline-variant/30 flex items-center justify-between px-4 shrink-0">
<h3 class="text-label-bold font-label-bold text-on-surface uppercase tracking-wider">Gate Allocations Timeline</h3>
</div>
<div class="flex-1 overflow-auto bg-surface-container-lowest relative flex flex-col">
<!-- Timeline Header -->
<div class="flex border-b border-outline-variant/30 sticky top-0 bg-surface-container-lowest/90 backdrop-blur z-10 h-8 shrink-0">
<div class="w-20 border-r border-outline-variant/30 flex items-center justify-center text-label-bold font-label-bold text-on-surface-variant">Gate</div>
<div class="flex-1 flex relative">
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">10:00</div>
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">11:00</div>
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">12:00</div>
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">13:00</div>
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">14:00</div>
<div class="flex-1 border-r border-outline-variant/30 flex items-center pl-2 text-label-bold text-on-surface-variant">15:00</div>
<!-- Current Time Indicator -->
<div class="absolute top-0 bottom-0 w-px bg-error z-20" style="left: 65%;">
<div class="absolute -top-1 -translate-x-1/2 bg-error text-on-error text-[9px] px-1 rounded font-bold">13:30</div>
</div>
</div>
</div>
<!-- Gantt Rows -->
<div class="flex-1 flex flex-col relative min-w-[700px] p-2 gap-1">
<!-- Grid Lines Background -->
<div class="absolute inset-0 flex pointer-events-none z-0 ml-20">
<div class="flex-1 border-r border-outline-variant/20"></div>
<div class="flex-1 border-r border-outline-variant/20"></div>
<div class="flex-1 border-r border-outline-variant/20"></div>
<div class="flex-1 border-r border-outline-variant/20"></div>
<div class="flex-1 border-r border-outline-variant/20"></div>
<div class="flex-1"></div>
</div>
<!-- Row 1 (Highlighted) -->
<div class="flex h-[36px] bg-[#10B981]/5 border border-[#10B981]/20 rounded-lg group transition-colors z-10 relative items-center">
<div class="w-20 flex items-center justify-center text-label-bold text-primary font-bold z-20">G10</div>
<div class="flex-1 relative h-full">
<div class="absolute top-1 bottom-1 bg-gradient-to-r from-[#10B981] to-[#059669] rounded-md flex items-center px-3 shadow text-white text-data-mono cursor-pointer hover:brightness-110 overflow-hidden ring-2 ring-primary/30 z-30" style="left: 10%; right: 40%;">
<span class="relative z-10 font-bold">AA123</span>
<span class="material-symbols-outlined text-[14px] absolute right-2 top-1/2 -translate-y-1/2 opacity-70">diamond</span>
</div>
</div>
</div>
<!-- Row 2 -->
<div class="flex h-[36px] rounded-lg group hover:bg-surface-container/50 transition-colors z-10 relative items-center">
<div class="w-20 flex items-center justify-center text-label-bold text-on-surface-variant z-20">G11</div>
<div class="flex-1 relative h-full">
<div class="absolute top-1 bottom-1 bg-gradient-to-r from-[#F59E0B] to-[#D97706] rounded-md flex items-center px-3 shadow text-white text-data-mono cursor-pointer hover:brightness-110 overflow-hidden z-20" style="left: 5%; right: 35%;">
<span class="relative z-10 font-bold">UA456</span>
</div>
</div>
</div>
<!-- Row 3 -->
<div class="flex h-[36px] rounded-lg group hover:bg-surface-container/50 transition-colors z-10 relative items-center">
<div class="w-20 flex items-center justify-center text-label-bold text-on-surface-variant z-20">G12</div>
<div class="flex-1 relative h-full">
<div class="absolute top-1 bottom-1 bg-gradient-to-r from-[#10B981] to-[#059669] rounded-md flex items-center px-3 shadow text-white text-data-mono cursor-pointer hover:brightness-110 overflow-hidden z-20" style="left: 20%; right: 10%;">
<span class="relative z-10 font-bold">SW102</span>
</div>
</div>
</div>
<!-- Row 4 -->
<div class="flex h-[36px] rounded-lg bg-error-container/10 border border-error/10 group hover:bg-surface-container/50 transition-colors z-10 relative items-center">
<div class="w-20 flex items-center justify-center text-label-bold text-error z-20">G13</div>
<div class="flex-1 relative h-full">
<div class="absolute top-1 bottom-1 bg-gradient-to-r from-error to-[#991B1B] rounded-md flex items-center px-3 shadow text-white text-data-mono cursor-pointer hover:brightness-110 overflow-hidden z-30" style="left: 45%; right: 25%;">
<span class="relative z-10 font-bold">BA204</span>
</div>
</div>
</div>
</div>
</div>
</div>
</main>
<!-- Right Sidebar - Gate Details & Camera -->
<aside class="w-[340px] bg-surface-container-lowest shadow-sm flex flex-col overflow-hidden shrink-0 hidden lg:flex border-l border-outline-variant/30">
<div class="h-[56px] bg-surface-container-lowest border-b border-outline-variant/30 flex items-center justify-between px-5 shrink-0">
<h3 class="text-headline-md font-headline-md text-primary flex items-center gap-2">
                    Gate G10 Detail
                </h3>
<button class="text-on-surface-variant hover:bg-surface-container p-1.5 rounded-lg transition-colors"><span class="material-symbols-outlined text-[20px]">close</span></button>
</div>
<div class="flex-1 overflow-y-auto">
<!-- Live Camera Placeholder -->
<div class="p-4 border-b border-outline-variant/30">
<div class="relative w-full aspect-video bg-inverse-surface rounded-xl overflow-hidden shadow-inner flex items-center justify-center">
<div class="absolute top-2 left-2 bg-error text-white text-[10px] font-bold px-1.5 py-0.5 rounded flex items-center gap-1 shadow-sm">
<div class="w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div> LIVE
                        </div>
<span class="material-symbols-outlined text-outline-variant text-[48px] opacity-50">videocam</span>
<div class="absolute bottom-2 right-2 text-outline-variant text-[10px] font-data-mono">CAM_G10_RAMP</div>
</div>
</div>
<div class="p-5 border-b border-outline-variant/30">
<div class="flex justify-between items-start mb-2">
<div>
<div class="text-headline-lg font-headline-lg text-primary">AA123</div>
<div class="text-body-sm text-on-surface-variant">B737-800</div>
</div>
<div class="bg-[#10B981]/10 text-[#059669] border border-[#10B981]/30 text-[11px] font-bold px-2 py-1 rounded-md uppercase tracking-wide">On Time</div>
</div>
<div class="flex justify-between text-data-mono text-sm mt-4 bg-surface-container-low p-3 rounded-xl border border-outline-variant/30">
<div class="text-center">
<div class="text-on-surface-variant text-[10px] uppercase font-bold tracking-wider">ARR (JFK)</div>
<div class="text-on-surface font-bold mt-1">10:45</div>
</div>
<div class="flex items-center text-outline-variant">
<span class="material-symbols-outlined text-[16px]">arrow_forward</span>
</div>
<div class="text-center">
<div class="text-on-surface-variant text-[10px] uppercase font-bold tracking-wider">DEP (LAX)</div>
<div class="text-on-surface font-bold mt-1">14:30</div>
</div>
</div>
</div>
<div class="p-5">
<h4 class="text-label-bold font-label-bold text-on-surface-variant uppercase mb-4 tracking-wider">Turnaround Progress</h4>
<div class="space-y-4">
<!-- Catering -->
<div class="flex items-start gap-4">
<div class="w-7 h-7 rounded-full bg-[#10B981]/20 flex items-center justify-center shrink-0 border border-[#10B981]/30">
<span class="material-symbols-outlined text-[16px] text-[#059669]">check</span>
</div>
<div class="flex-1 pt-1">
<div class="flex justify-between items-center">
<span class="text-body-md font-semibold text-on-surface">Catering</span>
<span class="text-[11px] font-bold text-[#059669] uppercase tracking-wide">Completed</span>
</div>
<div class="text-[12px] text-on-surface-variant mt-1">LSG Sky Chefs • 11:15</div>
</div>
</div>
<!-- Fueling -->
<div class="flex items-start gap-4">
<div class="w-7 h-7 rounded-full bg-[#F59E0B]/20 flex items-center justify-center shrink-0 border border-[#F59E0B]/40 relative">
<div class="w-3 h-3 rounded-full bg-[#D97706] animate-pulse"></div>
</div>
<div class="flex-1 pt-1">
<div class="flex justify-between items-center">
<span class="text-body-md font-semibold text-on-surface">Fueling</span>
<span class="text-[11px] font-bold text-[#D97706] uppercase tracking-wide">In Progress</span>
</div>
<div class="w-full bg-surface-container-highest h-2 rounded-full mt-2 overflow-hidden shadow-inner">
<div class="bg-gradient-to-r from-[#F59E0B] to-[#D97706] h-full rounded-full" style="width: 75%"></div>
</div>
<div class="text-[12px] text-on-surface-variant mt-1.5 flex justify-between font-data-mono">
<span>Req: 12,500L</span>
<span class="font-bold text-[#D97706]">75%</span>
</div>
</div>
</div>
<!-- Cleaning -->
<div class="flex items-start gap-4">
<div class="w-7 h-7 rounded-full border-2 border-outline-variant/40 flex items-center justify-center shrink-0 bg-surface-container-lowest">
</div>
<div class="flex-1 pt-1">
<div class="flex justify-between items-center">
<span class="text-body-md font-semibold text-on-surface">Cleaning</span>
<span class="text-[11px] font-bold text-on-surface-variant uppercase tracking-wide">Pending</span>
</div>
<div class="text-[12px] text-on-surface-variant mt-1">Crew dispatched</div>
</div>
</div>
<!-- Boarding -->
<div class="flex items-start gap-4">
<div class="w-7 h-7 rounded-full border-2 border-outline-variant/40 flex items-center justify-center shrink-0 bg-surface-container-lowest">
</div>
<div class="flex-1 pt-1">
<div class="flex justify-between items-center">
<span class="text-body-md font-semibold text-on-surface">Boarding</span>
<span class="text-[11px] font-bold text-on-surface-variant uppercase tracking-wide">Sch: 14:15</span>
</div>
</div>
</div>
</div>
<div class="mt-8">
<button class="w-full bg-primary text-on-primary h-10 rounded-xl text-label-bold font-label-bold shadow-md hover:shadow-lg hover:-translate-y-0.5 transition-all flex items-center justify-center gap-2">
<span class="material-symbols-outlined text-[18px]">edit_document</span>
                            Log Manual Action
                        </button>
</div>
</div>
</div>
</aside>
</div>
</body></html>