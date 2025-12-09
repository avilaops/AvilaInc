export interface BlueprintBullet {
    title: string;
    description: string;
    details?: string[];
}
export interface BlueprintSection {
    id: string;
    title: string;
    summary: string;
    highlights: BlueprintBullet[];
}
export declare const devtoolsBlueprint: BlueprintSection[];
export declare const devtoolsChecklist: string[];
//# sourceMappingURL=devtoolsBlueprint.d.ts.map