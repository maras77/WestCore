import { Photo } from './Photo';

export interface User {
    id: string;
    userName: string;
    knownAs: number;
    gender: string;
    age: number;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
    roles?: string[];
}
