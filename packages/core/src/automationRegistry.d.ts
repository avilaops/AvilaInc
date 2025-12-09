export interface AutomationCapability {
    id: string;
    name: string;
    description: string;
    package?: string;
    docs?: string;
    tags?: string[];
    status: 'planned' | 'in-progress' | 'available';
}
export interface AutomationCategory {
    id: string;
    label: string;
    icon?: string;
    capabilities: AutomationCapability[];
}
export declare const automationCatalog: AutomationCategory[];
export declare const automationCapabilities: {
    id: string;
    name: string;
    description: string;
    package?: string;
    docs?: string;
    tags?: string[];
    status: "planned" | "in-progress" | "available";
    category: string;
    icon: string | undefined;
}[];
//# sourceMappingURL=automationRegistry.d.ts.map