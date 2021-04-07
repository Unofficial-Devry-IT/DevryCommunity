export interface CreateCourseTypeOption
{
    courseNumber: string;
    courseName: string;
    courseCategory: string;
    description: string;
    voiceChannels: string[];
    textChannels: string[];
}

export interface CreatePublicTypeOption
{
    name: string;
}

export interface CreateResourceTypeOption
{
    name: string;
    requireRole: boolean;
    roleName: string;
}