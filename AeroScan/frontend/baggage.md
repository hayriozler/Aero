<!DOCTYPE html>

<html class="dark" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>BRS COMMAND - Flight Manifest</title>
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
        h1, h2, h3, .font-headline { font-family: 'Space Grotesk', sans-serif; }
    </style>
</head>
<body class="bg-background text-on-surface flex h-screen overflow-hidden">
<!-- SideNavBar Shell -->
<aside class="bg-[#171a1c] flex flex-col h-screen py-4 docked left-0 h-full w-64 no-border shrink-0">
<div class="px-6 mb-8">
<div class="flex items-center gap-3">
<div class="w-8 h-8 bg-primary-container flex items-center justify-center rounded-lg">
<span class="material-symbols-outlined text-on-primary-container" style="font-variation-settings: 'FILL' 1;">terminal</span>
</div>
<div>
<div class="font-headline uppercase text-xs font-medium text-[#FFB800]">Terminal 5</div>
<div class="text-[10px] text-[#eeeef0]/50 tracking-widest">LIVE OPS - UTC</div>
</div>
</div>
</div>
<nav class="flex-1 space-y-1">
<a class="flex items-center px-6 py-3 space-x-4 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined">dashboard</span>
<span>Dashboard</span>
</a>
<a class="flex items-center px-6 py-3 space-x-4 text-[#FFB800] border-l-4 border-[#FFB800] bg-[#232629] transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">flight_takeoff</span>
<span>Flights</span>
</a>
<a class="flex items-center px-6 py-3 space-x-4 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined">barcode_scanner</span>
<span>Scanning</span>
</a>
<a class="flex items-center px-6 py-3 space-x-4 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined">assessment</span>
<span>Reports</span>
</a>
</nav>
<div class="px-6 mt-auto space-y-1">
<a class="flex items-center py-3 space-x-4 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined">settings</span>
<span>Settings</span>
</a>
<a class="flex items-center py-3 space-x-4 text-[#eeeef0]/50 hover:text-[#FFB800] hover:bg-[#232629]/50 transition-all duration-200 ease-in-out font-headline uppercase text-xs font-medium" href="#">
<span class="material-symbols-outlined">help</span>
<span>Support</span>
</a>
<button class="w-full mt-4 py-3 bg-secondary text-on-secondary font-headline font-bold text-[10px] tracking-tighter uppercase rounded hover:bg-secondary-dim active:scale-95 transition-all">
                EMERGENCY STOP
            </button>
</div>
</aside>
<!-- Main Content Canvas -->
<main class="flex-1 flex flex-col min-w-0 bg-background overflow-hidden relative">
<!-- TopNavBar Shell -->
<header class="bg-[#0c0e10] flex justify-between items-center w-full px-6 h-16 shadow-[0px_2px_8px_rgba(0,0,0,0.5)] z-10">
<div class="flex items-center gap-8">
<span class="text-xl font-bold uppercase text-[#eeeef0] font-['Space_Grotesk'] tracking-tight">BRS COMMAND</span>
<nav class="hidden md:flex gap-6 h-full items-center">
<span class="text-[#FFB800] border-b-2 border-[#FFB800] h-16 flex items-center px-2 cursor-pointer font-headline text-sm font-medium">MANIFEST</span>
<span class="text-[#eeeef0]/60 hover:bg-[#232629] transition-colors h-16 flex items-center px-2 cursor-pointer font-headline text-sm font-medium">LIVE STREAM</span>
<span class="text-[#eeeef0]/60 hover:bg-[#232629] transition-colors h-16 flex items-center px-2 cursor-pointer font-headline text-sm font-medium">HISTORY</span>
</nav>
</div>
<div class="flex items-center gap-4">
<div class="flex items-center gap-2 bg-surface-container-highest px-3 py-1.5 rounded-lg border border-outline-variant/15">
<span class="material-symbols-outlined text-xs text-primary">sensors</span>
<span class="text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">System Link: Active</span>
</div>
<div class="flex items-center gap-1">
<button class="p-2 text-[#eeeef0]/60 hover:bg-[#232629] rounded transition-colors">
<span class="material-symbols-outlined">schedule</span>
</button>
<button class="p-2 text-[#eeeef0]/60 hover:bg-[#232629] rounded transition-colors">
<span class="material-symbols-outlined">account_circle</span>
</button>
</div>
</div>
</header>
<!-- Telemetry Strip -->
<div class="h-1 w-full bg-surface-container overflow-hidden shrink-0">
<div class="h-full bg-gradient-to-r from-transparent via-primary to-transparent w-1/3 animate-[telemetry-scan_3s_infinite_linear]"></div>
</div>
<style>
            @keyframes telemetry-scan {
                0% { transform: translateX(-100%); }
                100% { transform: translateX(300%); }
            }
        </style>
<!-- Workspace Area -->
<div class="flex-1 overflow-y-auto p-6 space-y-6">
<!-- Page Header & Stats Bento -->
<div class="grid grid-cols-1 md:grid-cols-4 gap-4">
<div class="md:col-span-1 bg-surface-container p-4 flex flex-col justify-between">
<h1 class="text-3xl font-headline font-bold text-on-surface leading-none">FLIGHTS</h1>
<p class="text-xs text-on-surface-variant uppercase tracking-widest mt-1">Daily Manifest Summary</p>
</div>
<div class="bg-surface-container-high p-4 flex items-center gap-4">
<div class="w-10 h-10 rounded bg-primary/10 flex items-center justify-center">
<span class="material-symbols-outlined text-primary">airplane_ticket</span>
</div>
<div>
<div class="text-2xl font-headline font-bold leading-none">42</div>
<div class="text-[10px] text-on-surface-variant uppercase font-medium">Active Flights</div>
</div>
</div>
<div class="bg-surface-container-high p-4 flex items-center gap-4">
<div class="w-10 h-10 rounded bg-secondary/10 flex items-center justify-center">
<span class="material-symbols-outlined text-secondary">luggage</span>
</div>
<div>
<div class="text-2xl font-headline font-bold leading-none">8,421</div>
<div class="text-[10px] text-on-surface-variant uppercase font-medium">Bags Pending</div>
</div>
</div>
<div class="bg-surface-container-high p-4 flex items-center gap-4">
<div class="w-10 h-10 rounded bg-tertiary-container/10 flex items-center justify-center">
<span class="material-symbols-outlined text-tertiary-dim">warning</span>
</div>
<div>
<div class="text-2xl font-headline font-bold leading-none">12</div>
<div class="text-[10px] text-on-surface-variant uppercase font-medium">Mismatches</div>
</div>
</div>
</div>
<!-- Main Data Table Container -->
<section class="bg-surface-container shadow-2xl relative overflow-hidden flex flex-col">
<!-- Table Filters / Toolbar -->
<div class="p-4 bg-surface-container-high flex flex-wrap items-center justify-between gap-4">
<div class="flex flex-1 items-center gap-3 min-w-[300px]">
<div class="relative flex-1 max-w-md">
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-sm">search</span>
<input class="w-full bg-surface-container-highest border-none focus:ring-1 focus:ring-primary text-xs font-headline font-medium py-2.5 pl-10 pr-4 placeholder:text-on-surface-variant/40" placeholder="SEARCH FLIGHT #, GATE, OR AIRLINE..." type="text"/>
</div>
<button class="flex items-center gap-2 bg-surface-container-highest px-4 py-2 text-[10px] font-headline font-bold uppercase border border-outline-variant/15 hover:bg-surface-bright transition-colors">
<span class="material-symbols-outlined text-sm">filter_list</span>
                            Filter
                        </button>
</div>
<div class="flex items-center gap-2">
<span class="text-[10px] font-headline font-bold text-on-surface-variant mr-2">SORT BY: STD</span>
<div class="flex border border-outline-variant/15">
<button class="px-3 py-1.5 bg-primary text-on-primary-fixed font-headline font-bold text-[10px]">ALL</button>
<button class="px-3 py-1.5 hover:bg-surface-container-highest transition-colors font-headline font-bold text-[10px] text-on-surface-variant">DOMESTIC</button>
<button class="px-3 py-1.5 hover:bg-surface-container-highest transition-colors font-headline font-bold text-[10px] text-on-surface-variant">INTL</button>
</div>
</div>
</div>
<!-- Manifest Table -->
<div class="overflow-x-auto">
<table class="w-full text-left border-collapse">
<thead class="bg-surface-container-highest border-b border-outline-variant/5">
<tr>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Flight #</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Airline</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">STD</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Gate</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Total Bags</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Reconciled</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest">Status</th>
<th class="px-6 py-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase tracking-widest text-right">Action</th>
</tr>
</thead>
<tbody class="divide-y divide-outline-variant/5">
<!-- Row 1: Active Focus -->
<tr class="bg-surface-bright border-l-4 border-primary group transition-colors">
<td class="px-6 py-5">
<div class="font-headline font-bold text-lg text-primary leading-none">BA202</div>
<div class="text-[9px] text-on-surface-variant uppercase font-medium mt-0.5">London LHR</div>
</td>
<td class="px-6 py-5">
<div class="flex items-center gap-2">
<div class="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center overflow-hidden">
<img alt="British Airways Logo" class="w-full h-full object-cover" data-alt="clean minimalist British Airways speedbird logo on dark background" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBtzG_nbEX6EZuyC20mu38kPBe4c6bPrLNXZ9cRApOzkbqiY6t41K8E8r8NimVbUZ9AIWbc9qvxAGP51GY2U9Jx1ofoValHn5W7rw0qiF_E4AGJJybg8920IKV4EUl51U7rm0yuBmeNck_wpkB90vD9mziD2ILBFlOGCGBosxkUlU5cR776f61YhuD2-ZplVPxauku56KeL7GHLocEIRzC28TSV4--Kkc_E7lzBtCcadLFOdT3ZM9U3xCpw-y-roSwtpPTOfSIvDvE"/>
</div>
<span class="text-xs font-medium uppercase text-on-surface">British Airways</span>
</div>
</td>
<td class="px-6 py-5 font-headline font-bold text-sm">14:20</td>
<td class="px-6 py-5 font-headline font-bold text-sm">A24</td>
<td class="px-6 py-5 font-headline font-bold text-sm">342</td>
<td class="px-6 py-5">
<div class="flex items-center gap-3">
<div class="flex-1 h-1.5 bg-surface-container-highest rounded-full overflow-hidden w-24">
<div class="h-full bg-primary" style="width: 88%;"></div>
</div>
<span class="text-xs font-headline font-bold">88%</span>
</div>
</td>
<td class="px-6 py-5">
<div class="inline-flex items-center gap-2 px-2.5 py-1 rounded bg-secondary/10 border border-secondary/20 shadow-[0_0_8px_rgba(255,117,36,0.2)]">
<span class="w-1.5 h-1.5 rounded-full bg-secondary animate-pulse"></span>
<span class="text-[9px] font-headline font-bold text-secondary uppercase tracking-wider">Boarding</span>
</div>
</td>
<td class="px-6 py-5 text-right">
<button class="bg-primary-container text-on-primary-fixed px-3 py-1.5 text-[10px] font-headline font-bold uppercase rounded hover:bg-primary-dim transition-all active:scale-95">Inspect</button>
</td>
</tr>
<!-- Row 2 -->
<tr class="hover:bg-surface-container-high transition-colors">
<td class="px-6 py-5">
<div class="font-headline font-bold text-lg leading-none">UA951</div>
<div class="text-[9px] text-on-surface-variant uppercase font-medium mt-0.5">New York JFK</div>
</td>
<td class="px-6 py-5">
<div class="flex items-center gap-2">
<div class="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center overflow-hidden">
<img alt="United Logo" class="w-full h-full object-cover" data-alt="clean minimalist United Airlines globe logo on navy blue background" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBZLss4JssRqkzX02OZvKPZl12PLyD9MWd6LGQpgl3BbdatybXM3Pj7uw_GFqwirDc3Y5b35t6qu1R91vh2j3SSXXaEo6XQUccEX5d2Ia5jStPKiPfnUwyabVCpVWn6h_EFxVR6nzlAoSAd9UugGZ0YXnKAHeFNJkyhT9WCMeqkEAyYu5z__Uofz568cMf2mS0S88SGhwnQfidHGp8IPEzqT-ZE2RihBfPSOhV3wU4eWlulYYHIHR7erHHkXrKgWulXdK3g-MKRn4I"/>
</div>
<span class="text-xs font-medium uppercase text-on-surface">United Airlines</span>
</div>
</td>
<td class="px-6 py-5 font-headline font-bold text-sm">14:45</td>
<td class="px-6 py-5 font-headline font-bold text-sm">B02</td>
<td class="px-6 py-5 font-headline font-bold text-sm">215</td>
<td class="px-6 py-5">
<div class="flex items-center gap-3">
<div class="flex-1 h-1.5 bg-surface-container-highest rounded-full overflow-hidden w-24">
<div class="h-full bg-error" style="width: 45%;"></div>
</div>
<span class="text-xs font-headline font-bold text-error">45%</span>
</div>
</td>
<td class="px-6 py-5">
<div class="inline-flex items-center gap-2 px-2.5 py-1 rounded bg-error/10 border border-error/20">
<span class="w-1.5 h-1.5 rounded-full bg-error"></span>
<span class="text-[9px] font-headline font-bold text-error uppercase tracking-wider">Final Call</span>
</div>
</td>
<td class="px-6 py-5 text-right">
<button class="bg-surface-container-highest text-on-surface px-3 py-1.5 text-[10px] font-headline font-bold uppercase rounded border border-outline-variant/30 hover:bg-surface-bright transition-all active:scale-95">Inspect</button>
</td>
</tr>
<!-- Row 3 -->
<tr class="hover:bg-surface-container-high transition-colors">
<td class="px-6 py-5">
<div class="font-headline font-bold text-lg leading-none">AF1202</div>
<div class="text-[9px] text-on-surface-variant uppercase font-medium mt-0.5">Paris CDG</div>
</td>
<td class="px-6 py-5">
<div class="flex items-center gap-2">
<div class="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center overflow-hidden">
<img alt="Air France Logo" class="w-full h-full object-cover" data-alt="clean minimalist Air France red accent logo on white background" src="https://lh3.googleusercontent.com/aida-public/AB6AXuB5tKBevEQZLCimz2d5Q0mDADV6AjrpxI0W0VR46Jrb2UnJmPLYaF0egIJmB0lw6jmGR1ilVgGi1cWjW9p_BeYgzSzYzIUDSob8TUMmGe3pWKJpNItYV4CVeIZv9nnMCepXHNlzybEgsHkVjq9A2G_g5x-8jjyKEBVLE_grDldpE3oOlApKXgXVnTpb1je53uQKnrmSCh5qXHuWm8Wvq3hWSNdGZmDCSsKwcNER4GzIoQVx_TNepYIfsde8Pi6m9yBznVG9vgXFjnw"/>
</div>
<span class="text-xs font-medium uppercase text-on-surface">Air France</span>
</div>
</td>
<td class="px-6 py-5 font-headline font-bold text-sm">15:10</td>
<td class="px-6 py-5 font-headline font-bold text-sm">C11</td>
<td class="px-6 py-5 font-headline font-bold text-sm">189</td>
<td class="px-6 py-5">
<div class="flex items-center gap-3">
<div class="flex-1 h-1.5 bg-surface-container-highest rounded-full overflow-hidden w-24">
<div class="h-full bg-on-surface-variant" style="width: 100%;"></div>
</div>
<span class="text-xs font-headline font-bold text-on-surface-variant">100%</span>
</div>
</td>
<td class="px-6 py-5">
<div class="inline-flex items-center gap-2 px-2.5 py-1 rounded bg-surface-container-highest border border-outline-variant/30">
<span class="w-1.5 h-1.5 rounded-full bg-on-surface-variant"></span>
<span class="text-[9px] font-headline font-bold text-on-surface-variant uppercase tracking-wider">Closed</span>
</div>
</td>
<td class="px-6 py-5 text-right">
<button class="bg-surface-container-highest text-on-surface px-3 py-1.5 text-[10px] font-headline font-bold uppercase rounded border border-outline-variant/30 hover:bg-surface-bright transition-all active:scale-95">View Details</button>
</td>
</tr>
<!-- Row 4 -->
<tr class="hover:bg-surface-container-high transition-colors">
<td class="px-6 py-5">
<div class="font-headline font-bold text-lg leading-none">EK008</div>
<div class="text-[9px] text-on-surface-variant uppercase font-medium mt-0.5">Dubai DXB</div>
</td>
<td class="px-6 py-5">
<div class="flex items-center gap-2">
<div class="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center overflow-hidden">
<img alt="Emirates Logo" class="w-full h-full object-cover" data-alt="clean minimalist Emirates airlines Arabic calligraphy logo in red on dark background" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAUutpH7Mt1jYsNJwYjLIk-SUc24v7Mt2ao_gbuFhk6pXLBcSEvckWG3U96idftxXACUjKU-NWrrsXgJm-phvizKT4mq8NxxiImO-YnfBA6FXDpoK99fpyMA_Mn0rj4jjvncOhxBvgHT5IXa5XP_Qpee7mFY4-0vVlEKEdAUx5VNzTAAJbpUfAYNlT2rvmsmTYI10EnceGQlU1a4qW0QCT2x7zuLx39nNquzZT7-8e8msVnWDn9q6dsDklPUrDJkbcDIvcagd0SxI8"/>
</div>
<span class="text-xs font-medium uppercase text-on-surface">Emirates</span>
</div>
</td>
<td class="px-6 py-5 font-headline font-bold text-sm">15:35</td>
<td class="px-6 py-5 font-headline font-bold text-sm">A01</td>
<td class="px-6 py-5 font-headline font-bold text-sm">512</td>
<td class="px-6 py-5">
<div class="flex items-center gap-3">
<div class="flex-1 h-1.5 bg-surface-container-highest rounded-full overflow-hidden w-24">
<div class="h-full bg-primary" style="width: 62%;"></div>
</div>
<span class="text-xs font-headline font-bold">62%</span>
</div>
</td>
<td class="px-6 py-5">
<div class="inline-flex items-center gap-2 px-2.5 py-1 rounded bg-secondary/10 border border-secondary/20 shadow-[0_0_8px_rgba(255,117,36,0.2)]">
<span class="w-1.5 h-1.5 rounded-full bg-secondary"></span>
<span class="text-[9px] font-headline font-bold text-secondary uppercase tracking-wider">Boarding</span>
</div>
</td>
<td class="px-6 py-5 text-right">
<button class="bg-surface-container-highest text-on-surface px-3 py-1.5 text-[10px] font-headline font-bold uppercase rounded border border-outline-variant/30 hover:bg-surface-bright transition-all active:scale-95">Inspect</button>
</td>
</tr>
<!-- Row 5 -->
<tr class="hover:bg-surface-container-high transition-colors">
<td class="px-6 py-5">
<div class="font-headline font-bold text-lg leading-none">LH115</div>
<div class="text-[9px] text-on-surface-variant uppercase font-medium mt-0.5">Frankfurt FRA</div>
</td>
<td class="px-6 py-5">
<div class="flex items-center gap-2">
<div class="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center overflow-hidden">
<img alt="Lufthansa Logo" class="w-full h-full object-cover" data-alt="clean minimalist Lufthansa crane logo in navy blue circle on white background" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBS-dKnO7Izg8BuXl9OgB2ue9wHDeNlKVbjKh7PGlczPKalIG5UxGYSGpThHbJ8Ujb7QRTwAAyQOsMpMXmVHRPb4pFqn_-w9ZtKRTcB3cHPD136tQ0gsmBn8OxFGNoeH4DeduGI0_akSvBCstogVXHMubBOwSSMxTGkkTbOQqoRdPzL9JTVlzPuMrSqoGvZby96A9gdMmibhcoSoaxfujowm-sKsIewMn3LBuqPFEhUYaVfd2_sB5ny_rDhYwW0Vgoo-zMXvWDn8es"/>
</div>
<span class="text-xs font-medium uppercase text-on-surface">Lufthansa</span>
</div>
</td>
<td class="px-6 py-5 font-headline font-bold text-sm">16:05</td>
<td class="px-6 py-5 font-headline font-bold text-sm">B18</td>
<td class="px-6 py-5 font-headline font-bold text-sm">224</td>
<td class="px-6 py-5">
<div class="flex items-center gap-3">
<div class="flex-1 h-1.5 bg-surface-container-highest rounded-full overflow-hidden w-24">
<div class="h-full bg-primary" style="width: 12%;"></div>
</div>
<span class="text-xs font-headline font-bold">12%</span>
</div>
</td>
<td class="px-6 py-5">
<div class="inline-flex items-center gap-2 px-2.5 py-1 rounded bg-surface-container-highest border border-outline-variant/30">
<span class="w-1.5 h-1.5 rounded-full bg-outline-variant"></span>
<span class="text-[9px] font-headline font-bold text-on-surface-variant uppercase tracking-wider">Scheduled</span>
</div>
</td>
<td class="px-6 py-5 text-right">
<button class="bg-surface-container-highest text-on-surface px-3 py-1.5 text-[10px] font-headline font-bold uppercase rounded border border-outline-variant/30 hover:bg-surface-bright transition-all active:scale-95">Inspect</button>
</td>
</tr>
</tbody>
</table>
</div>
<!-- Pagination / Footer -->
<div class="p-4 bg-surface-container-high border-t border-outline-variant/5 flex items-center justify-between">
<span class="text-[10px] font-headline font-bold text-on-surface-variant uppercase">Showing 1-5 of 42 Records</span>
<div class="flex gap-1">
<button class="p-2 bg-surface-container-highest border border-outline-variant/30 hover:bg-surface-bright transition-colors">
<span class="material-symbols-outlined text-sm">chevron_left</span>
</button>
<button class="px-3 py-1 bg-primary text-on-primary-fixed font-headline font-bold text-[10px]">1</button>
<button class="px-3 py-1 hover:bg-surface-container-highest transition-colors font-headline font-bold text-[10px]">2</button>
<button class="px-3 py-1 hover:bg-surface-container-highest transition-colors font-headline font-bold text-[10px]">3</button>
<button class="p-2 bg-surface-container-highest border border-outline-variant/30 hover:bg-surface-bright transition-colors">
<span class="material-symbols-outlined text-sm">chevron_right</span>
</button>
</div>
</div>
</section>
<!-- Bottom Context Cards -->
<div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
<!-- System Alerts Card -->
<div class="lg:col-span-1 bg-surface-container-highest p-6 relative group overflow-hidden">
<div class="absolute top-0 left-0 w-full h-1 bg-error/50"></div>
<div class="flex justify-between items-start mb-4">
<h3 class="font-headline font-bold text-sm uppercase tracking-widest text-error">Critical Alerts</h3>
<span class="material-symbols-outlined text-error" style="font-variation-settings: 'FILL' 1;">error</span>
</div>
<div class="space-y-4">
<div class="flex gap-4 p-3 bg-error/5 border-l-2 border-error">
<div class="text-[10px] font-headline font-bold text-error mt-0.5">14:12</div>
<div>
<div class="text-xs font-bold text-on-surface uppercase">Gate B02 Conveyor Stall</div>
<div class="text-[10px] text-on-surface-variant mt-1">Impacts Flight UA951 Reconciler. Maintenance dispatched.</div>
</div>
</div>
<div class="flex gap-4 p-3 bg-secondary/5 border-l-2 border-secondary">
<div class="text-[10px] font-headline font-bold text-secondary mt-0.5">14:05</div>
<div>
<div class="text-xs font-bold text-on-surface uppercase">Unscannable Tag Detected</div>
<div class="text-[10px] text-on-surface-variant mt-1">Terminal 5, Pier D. Manual reconciliation required.</div>
</div>
</div>
</div>
</div>
<!-- Data Visualizer Card -->
<div class="lg:col-span-2 bg-surface-container p-6 relative overflow-hidden flex flex-col justify-between">
<div class="flex justify-between items-center mb-6">
<h3 class="font-headline font-bold text-sm uppercase tracking-widest text-on-surface">Throughput Density</h3>
<div class="flex items-center gap-4 text-[10px] font-headline font-bold text-on-surface-variant uppercase">
<div class="flex items-center gap-1.5">
<span class="w-2 h-2 rounded-full bg-primary"></span>
                                Reconciled
                            </div>
<div class="flex items-center gap-1.5">
<span class="w-2 h-2 rounded-full bg-surface-container-highest"></span>
                                Total Flow
                            </div>
</div>
</div>
<div class="h-32 flex items-end justify-between gap-1">
<div class="w-full bg-surface-container-highest h-[40%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[60%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[60%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[85%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[90%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[95%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[55%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[40%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[30%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[20%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[75%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[90%] group-hover:bg-primary-dim transition-colors"></div>
</div>
<div class="w-full bg-surface-container-highest h-[100%] rounded-t-sm relative group">
<div class="absolute bottom-0 left-0 w-full bg-primary h-[98%] group-hover:bg-primary-dim transition-colors"></div>
</div>
</div>
<div class="flex justify-between mt-2 text-[8px] font-headline font-bold text-on-surface-variant uppercase tracking-tighter">
<span>12:00</span>
<span>13:00</span>
<span>14:00</span>
<span>15:00</span>
<span>16:00</span>
<span>17:00</span>
<span>18:00</span>
</div>
</div>
</div>
</div>
<!-- Float FAB Context -->
<button class="fixed bottom-8 right-8 w-14 h-14 bg-primary-container text-on-primary-fixed rounded shadow-[0_8px_24px_rgba(255,184,0,0.25)] flex items-center justify-center hover:scale-105 active:scale-95 transition-transform z-20">
<span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">add</span>
</button>
</main>
</body></html>