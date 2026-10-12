<!DOCTYPE html>

<html class="light" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Resource Management: Counter Allocation - AODB Tactical Hub</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700;900&amp;family=JetBrains+Mono:wght@500&amp;display=swap" rel="stylesheet"/>
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
                            "on-primary": "#ffffff",
                            // Explicit standard statuses added for tactical UI feel if needed, otherwise relying on sys colors
                            "status-emerald": "#10b981",
                            "status-amber": "#f59e0b",
                            "status-crimson": "#e11d48",
                            "status-slate": "#64748b"
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
                            "label-bold": ["Inter", "sans-serif"],
                            "headline-md": ["Inter", "sans-serif"],
                            "body-md": ["Inter", "sans-serif"],
                            "headline-lg": ["Inter", "sans-serif"],
                            "data-mono": ["JetBrains Mono", "monospace"],
                            "body-sm": ["Inter", "sans-serif"]
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
        /* Base tactical styling */
        body {
            background-color: #F1F5F9; /* As per spec for canvas */
            color: #1b1b1d;
        }
        
        .tactical-card {
            background-color: #ffffff;
            border: 1px solid #E2E8F0;
            border-radius: 0.125rem;
        }
        
        .tactical-table th {
            border-bottom: 1px solid #E2E8F0;
            background-color: #f8fafc;
            position: sticky;
            top: 0;
            z-index: 10;
        }
        
        .tactical-table td {
            border: 1px solid #E2E8F0;
            height: 32px; /* Dense rows */
        }
        
        .status-chip-open { background-color: #10b981; color: white; }
        .status-chip-closed { background-color: #64748b; color: white; }
        .status-chip-maint { background-color: #e11d48; color: white; }

        /* Custom scrollbar for data density areas */
        ::-webkit-scrollbar {
            width: 8px;
            height: 8px;
        }
        ::-webkit-scrollbar-track {
            background: #f1f1f1; 
        }
        ::-webkit-scrollbar-thumb {
            background: #cbd5e1; 
            border-radius: 4px;
        }
        ::-webkit-scrollbar-thumb:hover {
            background: #94a3b8; 
        }
    </style>
</head>
<body class="flex flex-col h-screen overflow-hidden antialiased font-body-md text-body-md bg-[#F1F5F9]">
<!-- TopNavBar (Shared Component) -->
<header class="flex justify-between items-center w-full px-container-padding h-12 z-50 bg-surface-container-highest border-b border-outline-variant docked full-width top-0">
<div class="flex items-center gap-4">
<h1 class="text-headline-lg font-headline-lg font-black text-primary">AODB Tactical Hub</h1>
<nav class="hidden md:flex items-center gap-6 h-full ml-8">
<!-- Navigation derived from BottomNavBar/SideNavBar context -->
<a class="text-on-surface-variant text-label-bold font-label-bold h-full flex items-center hover:bg-surface-variant transition-colors px-2" href="#">Terminal Views</a>
<a class="text-primary font-bold border-b-2 border-primary text-label-bold font-label-bold h-full flex items-center hover:bg-surface-variant transition-colors px-2 opacity-80" href="#">Resource Management</a>
<a class="text-on-surface-variant text-label-bold font-label-bold h-full flex items-center hover:bg-surface-variant transition-colors px-2" href="#">Alerts</a>
</nav>
</div>
<div class="flex items-center gap-container-padding text-primary">
<!-- Search bar on left logic represented here if it were a full comp, keeping trailing icons -->
<button class="hover:bg-surface-variant transition-colors p-1 rounded"><span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">flight_takeoff</span></button>
<button class="hover:bg-surface-variant transition-colors p-1 rounded"><span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">percent</span></button>
<button class="hover:bg-surface-variant transition-colors p-1 rounded"><span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">cloud</span></button>
<button class="hover:bg-surface-variant transition-colors p-1 rounded"><span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">schedule</span></button>
</div>
</header>
<div class="flex flex-1 overflow-hidden">
<!-- SideNavBar (Shared Component) - Visible on Desktop -->
<aside class="hidden md:flex flex-col w-64 bg-surface h-full border-r border-outline-variant fixed left-0 top-0 pt-12 z-40 transition-all duration-150 ease-in-out">
<div class="p-container-padding border-b border-outline-variant">
<div class="flex items-center gap-3 mb-2">
<div class="w-8 h-8 rounded bg-primary-fixed-dim flex items-center justify-center text-on-primary-fixed font-label-bold text-label-bold">
                        AA
                    </div>
<div>
<div class="text-label-bold font-label-bold text-primary">Control Tower</div>
<div class="text-body-sm font-body-sm text-on-surface-variant">Terminal 1</div>
</div>
</div>
</div>
<nav class="flex-1 py-4 flex flex-col gap-1">
<a class="flex items-center gap-3 px-container-padding py-2 text-on-surface-variant hover:bg-surface-container-low transition-colors group" href="#">
<span class="material-symbols-outlined text-[20px] group-hover:text-primary transition-colors" style="font-variation-settings: 'FILL' 0;">domain</span>
<span class="text-label-bold font-label-bold">Terminal Views</span>
</a>
<a class="flex items-center gap-3 px-container-padding py-2 bg-secondary-container text-on-secondary-container border-r-4 border-primary transition-colors" href="#">
<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 1;">construction</span>
<span class="text-label-bold font-label-bold">Resource Management</span>
</a>
<a class="flex items-center gap-3 px-container-padding py-2 text-on-surface-variant hover:bg-surface-container-low transition-colors group" href="#">
<span class="material-symbols-outlined text-[20px] group-hover:text-primary transition-colors" style="font-variation-settings: 'FILL' 0;">warning</span>
<span class="text-label-bold font-label-bold">Alerts</span>
</a>
</nav>
<div class="border-t border-outline-variant py-2">
<a class="flex items-center gap-3 px-container-padding py-2 text-on-surface-variant hover:bg-surface-container-low transition-colors" href="#">
<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">settings</span>
<span class="text-label-bold font-label-bold">Settings</span>
</a>
<a class="flex items-center gap-3 px-container-padding py-2 text-on-surface-variant hover:bg-surface-container-low transition-colors" href="#">
<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">help</span>
<span class="text-label-bold font-label-bold">Support</span>
</a>
</div>
</aside>
<!-- Main Content Area -->
<main class="flex-1 md:ml-64 p-gutter overflow-hidden flex flex-col gap-card-gap">
<!-- Page Header & Action -->
<div class="flex justify-between items-center mb-1">
<div>
<h2 class="text-headline-md font-headline-md text-primary">Counter Allocation</h2>
<p class="text-body-sm font-body-sm text-on-surface-variant">Manage check-in counters and equipment assignments.</p>
</div>
<button class="bg-primary text-on-primary h-[28px] px-3 flex items-center gap-1 rounded hover:bg-surface-tint transition-colors text-label-bold font-label-bold shadow-sm" onclick="document.getElementById('add-counter-modal').classList.remove('hidden')">
<span class="material-symbols-outlined text-[16px]" style="font-variation-settings: 'FILL' 0;">add</span>
                    Add New Counter
                </button>
</div>
<!-- Bento Grid Layout for Data Density -->
<div class="grid grid-cols-1 lg:grid-cols-12 gap-card-gap flex-1 min-h-0">
<!-- Counter List Table (Spans 8 cols) -->
<div class="tactical-card lg:col-span-8 flex flex-col h-full overflow-hidden">
<div class="h-[36px] px-container-padding flex justify-between items-center border-b border-outline-variant bg-surface-bright">
<span class="text-label-bold font-label-bold text-on-surface uppercase tracking-wider">Active Counters (Terminal 1)</span>
<div class="flex gap-2">
<div class="relative">
<span class="material-symbols-outlined absolute left-2 top-1/2 -translate-y-1/2 text-on-surface-variant text-[16px]">search</span>
<input class="h-[24px] pl-7 pr-2 text-body-sm font-body-sm border border-outline-variant rounded focus:border-primary focus:ring-0 w-48 bg-surface" placeholder="Filter ID or Flight..." type="text"/>
</div>
<button class="text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[18px]">more_vert</span></button>
</div>
</div>
<div class="flex-1 overflow-auto">
<table class="w-full tactical-table text-left border-collapse">
<thead>
<tr>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase w-16">ID</th>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase w-24">Area</th>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase w-24">Status</th>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase w-24">Equipment</th>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase">Current Assignment</th>
<th class="p-table-cell-padding text-label-bold font-label-bold text-on-surface-variant uppercase w-24">Time Rem.</th>
<th class="p-table-cell-padding w-10"></th>
</tr>
</thead>
<tbody class="text-body-sm font-body-sm bg-surface-container-lowest">
<!-- Row 1 -->
<tr class="hover:bg-surface-container-low transition-colors group">
<td class="p-table-cell-padding text-data-mono font-data-mono font-bold">C01</td>
<td class="p-table-cell-padding text-on-surface-variant">North A</td>
<td class="p-table-cell-padding">
<span class="inline-block px-2 py-0.5 text-[10px] font-label-bold status-chip-open rounded-sm uppercase tracking-wider">Open</span>
</td>
<td class="p-table-cell-padding text-on-surface-variant">Staffed</td>
<td class="p-table-cell-padding font-data-mono">BA112 LHR</td>
<td class="p-table-cell-padding text-on-surface-variant text-right">01:45</td>
<td class="p-table-cell-padding text-center">
<button class="opacity-0 group-hover:opacity-100 text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[16px]">edit</span></button>
</td>
</tr>
<!-- Row 2 -->
<tr class="hover:bg-surface-container-low transition-colors group">
<td class="p-table-cell-padding text-data-mono font-data-mono font-bold">C02</td>
<td class="p-table-cell-padding text-on-surface-variant">North A</td>
<td class="p-table-cell-padding">
<span class="inline-block px-2 py-0.5 text-[10px] font-label-bold status-chip-open rounded-sm uppercase tracking-wider">Open</span>
</td>
<td class="p-table-cell-padding text-on-surface-variant">Staffed</td>
<td class="p-table-cell-padding font-data-mono">BA112 LHR</td>
<td class="p-table-cell-padding text-on-surface-variant text-right">01:45</td>
<td class="p-table-cell-padding text-center">
<button class="opacity-0 group-hover:opacity-100 text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[16px]">edit</span></button>
</td>
</tr>
<!-- Row 3 -->
<tr class="hover:bg-surface-container-low transition-colors group">
<td class="p-table-cell-padding text-data-mono font-data-mono font-bold">C03</td>
<td class="p-table-cell-padding text-on-surface-variant">North A</td>
<td class="p-table-cell-padding">
<span class="inline-block px-2 py-0.5 text-[10px] font-label-bold status-chip-closed rounded-sm uppercase tracking-wider">Closed</span>
</td>
<td class="p-table-cell-padding text-on-surface-variant">Self-Service</td>
<td class="p-table-cell-padding text-outline italic">Unassigned</td>
<td class="p-table-cell-padding text-on-surface-variant text-right">--:--</td>
<td class="p-table-cell-padding text-center">
<button class="opacity-0 group-hover:opacity-100 text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[16px]">edit</span></button>
</td>
</tr>
<!-- Row 4 -->
<tr class="hover:bg-surface-container-low transition-colors group">
<td class="p-table-cell-padding text-data-mono font-data-mono font-bold">C04</td>
<td class="p-table-cell-padding text-on-surface-variant">South B</td>
<td class="p-table-cell-padding">
<span class="inline-block px-2 py-0.5 text-[10px] font-label-bold status-chip-maint rounded-sm uppercase tracking-wider">Maint</span>
</td>
<td class="p-table-cell-padding text-on-surface-variant">Bag Drop</td>
<td class="p-table-cell-padding text-error font-data-mono">Belt Jam Fault</td>
<td class="p-table-cell-padding text-on-surface-variant text-right">Est. 2h</td>
<td class="p-table-cell-padding text-center">
<button class="opacity-0 group-hover:opacity-100 text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[16px]">edit</span></button>
</td>
</tr>
<!-- Generating more rows for density -->
<script>
                                    for(let i=5; i<=20; i++) {
                                        const id = `C${i.toString().padStart(2, '0')}`;
                                        const status = i%7===0 ? 'status-chip-closed' : 'status-chip-open';
                                        const statusText = i%7===0 ? 'Closed' : 'Open';
                                        const eq = i%3===0 ? 'Self-Service' : 'Staffed';
                                        const flt = i%7===0 ? '<span class="text-outline italic">Unassigned</span>' : `<span class="font-data-mono">AF${100+i} CDG</span>`;
                                        const time = i%7===0 ? '--:--' : '02:15';
                                        
                                        document.write(`
                                        <tr class="hover:bg-surface-container-low transition-colors group">
                                            <td class="p-table-cell-padding text-data-mono font-data-mono font-bold">${id}</td>
                                            <td class="p-table-cell-padding text-on-surface-variant">South B</td>
                                            <td class="p-table-cell-padding">
                                                <span class="inline-block px-2 py-0.5 text-[10px] font-label-bold ${status} rounded-sm uppercase tracking-wider">${statusText}</span>
                                            </td>
                                            <td class="p-table-cell-padding text-on-surface-variant">${eq}</td>
                                            <td class="p-table-cell-padding">${flt}</td>
                                            <td class="p-table-cell-padding text-on-surface-variant text-right">${time}</td>
                                            <td class="p-table-cell-padding text-center">
                                                <button class="opacity-0 group-hover:opacity-100 text-on-surface-variant hover:text-primary"><span class="material-symbols-outlined text-[16px]">edit</span></button>
                                            </td>
                                        </tr>
                                        `);
                                    }
                                </script>
</tbody>
</table>
</div>
</div>
<!-- Right Column: Status Summary & Quick Actions (Spans 4 cols) -->
<div class="lg:col-span-4 flex flex-col gap-card-gap h-full">
<!-- Utilization Widget -->
<div class="tactical-card">
<div class="h-[36px] px-container-padding flex justify-between items-center border-b border-outline-variant bg-surface-bright">
<span class="text-label-bold font-label-bold text-on-surface uppercase tracking-wider">Current Utilization</span>
</div>
<div class="p-container-padding flex flex-col gap-3">
<div>
<div class="flex justify-between text-body-sm font-body-sm mb-1">
<span class="text-on-surface-variant">Overall Capacity</span>
<span class="font-data-mono text-primary">78%</span>
</div>
<div class="w-full bg-surface-container h-2 rounded-full overflow-hidden">
<div class="bg-primary h-full" style="width: 78%"></div>
</div>
</div>
<div class="grid grid-cols-2 gap-2 mt-2">
<div class="border border-outline-variant rounded p-2 text-center bg-surface-container-lowest">
<div class="text-headline-lg font-headline-lg font-data-mono text-status-emerald">16</div>
<div class="text-label-bold font-label-bold text-on-surface-variant uppercase">Open</div>
</div>
<div class="border border-outline-variant rounded p-2 text-center bg-surface-container-lowest">
<div class="text-headline-lg font-headline-lg font-data-mono text-status-slate">3</div>
<div class="text-label-bold font-label-bold text-on-surface-variant uppercase">Closed</div>
</div>
<div class="border border-outline-variant rounded p-2 text-center bg-surface-container-lowest col-span-2">
<div class="text-headline-lg font-headline-lg font-data-mono text-status-crimson">1</div>
<div class="text-label-bold font-label-bold text-on-surface-variant uppercase">Maintenance</div>
</div>
</div>
</div>
</div>
<!-- Gantt-style Mini View -->
<div class="tactical-card flex-1 flex flex-col min-h-[200px]">
<div class="h-[36px] px-container-padding flex justify-between items-center border-b border-outline-variant bg-surface-bright">
<span class="text-label-bold font-label-bold text-on-surface uppercase tracking-wider">Upcoming Allocations</span>
<span class="material-symbols-outlined text-[16px] text-on-surface-variant">calendar_view_week</span>
</div>
<div class="p-container-padding flex-1 overflow-auto flex flex-col gap-2 relative">
<!-- Timeline Header -->
<div class="flex text-label-bold font-label-bold text-outline border-b border-outline-variant pb-1">
<div class="w-12">ID</div>
<div class="flex-1 flex justify-between px-2">
<span>10:00</span>
<span>11:00</span>
<span>12:00</span>
</div>
</div>
<!-- Rows -->
<div class="flex items-center text-body-sm font-body-sm">
<div class="w-12 font-data-mono text-on-surface-variant">C01</div>
<div class="flex-1 relative h-6 bg-surface-container rounded-sm overflow-hidden">
<!-- Flight Brick -->
<div class="absolute left-0 w-2/3 h-full bg-secondary-fixed border border-secondary-fixed-dim text-[10px] font-label-bold flex items-center px-1 text-on-secondary-fixed-variant truncate">BA112 LHR</div>
</div>
</div>
<div class="flex items-center text-body-sm font-body-sm">
<div class="w-12 font-data-mono text-on-surface-variant">C02</div>
<div class="flex-1 relative h-6 bg-surface-container rounded-sm overflow-hidden">
<div class="absolute left-0 w-2/3 h-full bg-secondary-fixed border border-secondary-fixed-dim text-[10px] font-label-bold flex items-center px-1 text-on-secondary-fixed-variant truncate">BA112 LHR</div>
</div>
</div>
<div class="flex items-center text-body-sm font-body-sm">
<div class="w-12 font-data-mono text-on-surface-variant">C03</div>
<div class="flex-1 relative h-6 bg-surface-container rounded-sm overflow-hidden">
<div class="absolute left-1/4 w-1/2 h-full bg-tertiary-fixed border border-tertiary-fixed-dim text-[10px] font-label-bold flex items-center px-1 text-on-tertiary-fixed-variant truncate">LH400 FRA</div>
</div>
</div>
<div class="flex items-center text-body-sm font-body-sm">
<div class="w-12 font-data-mono text-on-surface-variant">C04</div>
<div class="flex-1 relative h-6 bg-surface-container rounded-sm overflow-hidden">
<div class="absolute left-0 w-full h-full bg-error-container border border-error text-[10px] font-label-bold flex items-center justify-center text-on-error-container truncate pattern-diagonal-lines-sm opacity-80">MAINTENANCE</div>
</div>
</div>
<div class="flex items-center text-body-sm font-body-sm">
<div class="w-12 font-data-mono text-on-surface-variant">C05</div>
<div class="flex-1 relative h-6 bg-surface-container rounded-sm overflow-hidden">
<div class="absolute left-1/2 w-1/2 h-full bg-primary-fixed border border-primary-fixed-dim text-[10px] font-label-bold flex items-center px-1 text-on-primary-fixed-variant truncate">AF105 CDG</div>
</div>
</div>
</div>
</div>
</div>
</div>
</main>
</div>
<!-- BottomNavBar (Shared Component) - Mobile Only -->
<nav class="md:hidden fixed bottom-0 left-0 w-full z-50 flex justify-start items-center bg-primary h-10 border-t border-outline shadow-md docked full-width">
<div class="bg-error text-on-error px-4 h-full flex items-center cursor-pointer transition-all hover:brightness-110 flex-1 justify-center gap-2">
<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">error</span>
<span class="text-label-bold font-label-bold">Exceptions</span>
</div>
<div class="text-on-primary-container px-4 h-full flex items-center opacity-80 cursor-pointer transition-all hover:brightness-110 flex-1 justify-center gap-2 border-l border-surface-tint">
<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: 'FILL' 0;">priority_high</span>
<span class="text-label-bold font-label-bold">Operational Alerts</span>
</div>
</nav>
<!-- Add Counter Modal Overlay -->
<div class="hidden fixed inset-0 z-[100] flex items-center justify-center bg-primary/50 backdrop-blur-sm p-4" id="add-counter-modal">
<div class="tactical-card w-full max-w-md shadow-lg flex flex-col">
<div class="h-[40px] px-container-padding flex justify-between items-center border-b border-outline-variant bg-surface-bright">
<span class="text-headline-md font-headline-md text-primary">Define New Counter</span>
<button class="text-on-surface-variant hover:text-error transition-colors" onclick="document.getElementById('add-counter-modal').classList.add('hidden')">
<span class="material-symbols-outlined text-[20px]">close</span>
</button>
</div>
<form class="p-container-padding flex flex-col gap-4">
<div class="flex flex-col gap-1">
<label class="text-label-bold font-label-bold text-on-surface-variant uppercase">Counter ID</label>
<!-- Simulated validation state -->
<div class="relative">
<input class="w-full h-8 px-2 text-data-mono font-data-mono border border-outline-variant rounded focus:border-primary focus:ring-1 focus:ring-primary outline-none border-l-2 border-l-status-emerald bg-surface-container-lowest" required="" type="text" value="C21"/>
<span class="material-symbols-outlined absolute right-2 top-1/2 -translate-y-1/2 text-status-emerald text-[16px]">check_circle</span>
</div>
</div>
<div class="flex flex-col gap-1">
<label class="text-label-bold font-label-bold text-on-surface-variant uppercase">Terminal Area</label>
<select class="w-full h-8 px-2 text-body-sm font-body-sm border border-outline-variant rounded focus:border-primary focus:ring-1 focus:ring-primary outline-none bg-surface-container-lowest">
<option>North A</option>
<option>North B</option>
<option>South A</option>
<option>South B</option>
<option>Central Annex</option>
</select>
</div>
<div class="flex flex-col gap-1">
<label class="text-label-bold font-label-bold text-on-surface-variant uppercase">Equipment Type</label>
<div class="grid grid-cols-2 gap-2">
<label class="flex items-center justify-center gap-2 border border-primary bg-primary-fixed text-on-primary-fixed h-8 rounded cursor-pointer">
<input checked="" class="sr-only" name="eq_type" type="radio"/>
<span class="material-symbols-outlined text-[16px]">person</span>
<span class="text-label-bold font-label-bold">Staffed</span>
</label>
<label class="flex items-center justify-center gap-2 border border-outline-variant bg-surface hover:bg-surface-container-low text-on-surface-variant h-8 rounded cursor-pointer transition-colors">
<input class="sr-only" name="eq_type" type="radio"/>
<span class="material-symbols-outlined text-[16px]">qr_code_scanner</span>
<span class="text-label-bold font-label-bold">Self-Service</span>
</label>
</div>
</div>
<div class="flex justify-end gap-2 mt-4 pt-4 border-t border-outline-variant">
<button class="px-4 h-8 text-label-bold font-label-bold text-on-surface-variant hover:bg-surface-container transition-colors rounded border border-outline-variant bg-surface" onclick="document.getElementById('add-counter-modal').classList.add('hidden')" type="button">Cancel</button>
<button class="px-4 h-8 text-label-bold font-label-bold text-on-primary bg-primary hover:bg-surface-tint transition-colors rounded shadow-sm" type="button">Allocate Resource</button>
</div>
</form>
</div>
</div>
</body></html>