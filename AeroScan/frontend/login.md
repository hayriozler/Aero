<!DOCTYPE html>

<html class="dark" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>AeroScan BRS - Secure Login</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@300;400;500;600;700&amp;family=Inter:wght@300;400;500;600&amp;display=swap" rel="stylesheet"/>
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
        .kinetic-scan {
            background: linear-gradient(90deg, transparent, rgba(254, 183, 0, 0.1), transparent);
            background-size: 200% 100%;
        }
    </style>
</head>
<body class="bg-background text-on-background font-body min-h-screen flex flex-col items-center justify-center p-6 relative overflow-hidden">
<!-- Background Texture -->
<div class="absolute inset-0 z-0 opacity-10 pointer-events-none">
<div class="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-surface-variant via-background to-background"></div>
<img class="w-full h-full object-cover mix-blend-overlay" data-alt="Technical blueprint of an airport terminal layout with geometric lines and data points in dark monochrome tones" src="https://lh3.googleusercontent.com/aida-public/AB6AXuDBpzKl0JrGkUNFgy6brCkF14_GHxkd_sTHrdxOgYviWdfEVatUkPxWmoMlEckuEZpV8TcMyiRhM6Fui3v9nXOVIQogoCvxuCSvFiV2KMPW7pdiRA3E94pZ2hLVhoH8slKzIodzKMNhfGYFDgEQHxqAMBWBK6Jy6SnG5csgsrzicz8YcyMkZw7-bGXYURoqte16QOP5OJuf8dMsacZ4axZW4u1B3vfLUley2smjfstJFWiljH81HcCK9U7djBhHM-CtvpbzgEI_QKM"/>
</div>
<!-- Main Container -->
<div class="relative z-10 w-full max-w-md">
<!-- Telemetry Strip -->
<div class="w-full h-1 bg-tertiary-container/20 relative overflow-hidden mb-8 rounded-full">
<div class="absolute inset-0 kinetic-scan animate-[scan_3s_linear_infinite]"></div>
</div>
<!-- Branding Section -->
<div class="text-center mb-10">
<div class="flex items-center justify-center mb-4">
<div class="w-12 h-12 bg-primary-container flex items-center justify-center rounded-lg shadow-[0px_0px_20px_rgba(255,184,0,0.3)]">
<span class="material-symbols-outlined text-on-primary-fixed text-3xl" data-icon="barcode_scanner">barcode_scanner</span>
</div>
</div>
<h1 class="font-headline text-3xl font-bold tracking-tight text-on-surface uppercase">AeroScan <span class="text-primary-container">BRS</span></h1>
<p class="font-headline text-xs tracking-[0.2em] text-on-surface-variant mt-2 font-medium">BAGGAGE RECONCILIATION SYSTEM</p>
</div>
<!-- Login Card -->
<div class="bg-surface-container border-l-4 border-primary-container p-8 shadow-2xl relative">
<!-- Glass Overlay Subtle Effect -->
<div class="absolute inset-0 bg-surface-variant/10 backdrop-blur-[2px] -z-10"></div>
<div class="flex justify-between items-center mb-8">
<h2 class="font-headline text-lg font-semibold text-on-surface">TERMINAL ACCESS</h2>
<div class="flex items-center gap-2">
<span class="w-2 h-2 rounded-full bg-secondary animate-pulse shadow-[0px_0px_8px_rgba(255,117,36,0.6)]"></span>
<span class="text-[10px] font-headline font-bold text-secondary uppercase tracking-widest">Secure Area</span>
</div>
</div>
<form class="space-y-6">
<!-- Operator ID -->
<div class="space-y-2">
<label class="block font-headline text-[10px] font-bold text-on-surface-variant uppercase tracking-widest px-1">Operator ID</label>
<div class="relative group">
<div class="absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant group-focus-within:text-primary-container transition-colors">
<span class="material-symbols-outlined text-lg" data-icon="account_circle">account_circle</span>
</div>
<input class="w-full bg-surface-container-highest border-none h-12 pl-12 pr-4 text-on-surface font-headline font-medium focus:ring-0 focus:bg-surface-bright transition-all placeholder:text-on-surface-variant/30 text-sm" placeholder="ENTER ID" type="text"/>
<div class="absolute bottom-0 left-0 h-[2px] w-0 bg-primary-container group-focus-within:w-full transition-all duration-300"></div>
</div>
</div>
<!-- Password -->
<div class="space-y-2">
<label class="block font-headline text-[10px] font-bold text-on-surface-variant uppercase tracking-widest px-1">Password</label>
<div class="relative group">
<div class="absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant group-focus-within:text-primary-container transition-colors">
<span class="material-symbols-outlined text-lg" data-icon="lock">lock</span>
</div>
<input class="w-full bg-surface-container-highest border-none h-12 pl-12 pr-4 text-on-surface font-headline font-medium focus:ring-0 focus:bg-surface-bright transition-all placeholder:text-on-surface-variant/30 text-sm" placeholder="••••••••" type="password"/>
<div class="absolute bottom-0 left-0 h-[2px] w-0 bg-primary-container group-focus-within:w-full transition-all duration-300"></div>
</div>
</div>
<!-- Submit -->
<button class="w-full bg-primary-container text-on-primary-fixed font-headline font-bold py-4 uppercase tracking-widest hover:bg-primary-dim transition-all active:scale-[0.98] shadow-[0px_4px_16px_rgba(254,183,0,0.2)] flex items-center justify-center gap-3" type="submit">
<span>Secure Login</span>
<span class="material-symbols-outlined" data-icon="login">login</span>
</button>
</form>
<!-- Bottom Options -->
<div class="mt-8 flex justify-center">
<button class="text-[10px] font-headline font-bold text-on-surface-variant hover:text-on-surface transition-colors uppercase tracking-widest underline decoration-outline-variant/30 underline-offset-4">
                    Forgot Credentials?
                </button>
</div>
</div>
<!-- Footer / Status -->
<div class="mt-12 flex flex-col items-center gap-4">
<div class="flex items-center gap-6 text-[10px] font-headline font-medium text-on-surface-variant/60 uppercase tracking-[0.15em]">
<div class="flex items-center gap-2">
<span class="material-symbols-outlined text-xs text-on-surface-variant" data-icon="language">language</span>
<span>Server: LHR-NODE-04</span>
</div>
<div class="flex items-center gap-2">
<span class="material-symbols-outlined text-xs text-on-surface-variant" data-icon="update">update</span>
<span>v4.2.0-STABLE</span>
</div>
</div>
<!-- Global Status Bar -->
<div class="bg-surface-container-low px-4 py-2 rounded-full flex items-center gap-3 border border-outline-variant/10">
<div class="flex items-center gap-2">
<div class="relative flex h-2 w-2">
<span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary opacity-40"></span>
<span class="relative inline-flex rounded-full h-2 w-2 bg-primary"></span>
</div>
<span class="text-[10px] font-headline font-bold text-on-surface uppercase tracking-widest">System Status: <span class="text-primary-container">Online</span></span>
</div>
<div class="h-3 w-[1px] bg-outline-variant/30"></div>
<div class="flex items-center gap-2">
<span class="material-symbols-outlined text-[14px]" data-icon="timer">timer</span>
<span class="text-[10px] font-headline font-medium text-on-surface-variant">UTC 14:22:09</span>
</div>
</div>
</div>
</div>
<!-- Decorative Elements -->
<div class="absolute bottom-10 left-10 opacity-20 pointer-events-none hidden lg:block">
<p class="font-headline text-[8px] text-on-surface-variant leading-relaxed">
            CRITICAL INFRASTRUCTURE PROTOCOL [A-99]<br/>
            UNAUTHORIZED ACCESS IS PROHIBITED<br/>
            ALL LOGINS ARE RECORDED AND GEOLOCATED
        </p>
</div>
<div class="absolute top-10 right-10 opacity-20 pointer-events-none hidden lg:block">
<div class="flex flex-col items-end gap-1">
<div class="w-32 h-[1px] bg-outline-variant"></div>
<div class="w-16 h-[1px] bg-outline-variant"></div>
<p class="font-headline text-[8px] text-on-surface-variant mt-2">TERMINAL_5_MAIN_HUB</p>
</div>
</div>
<style>
        @keyframes scan {
            0% { transform: translateX(-100%); }
            100% { transform: translateX(100%); }
        }
    </style>
</body></html>