import { Photo } from "./photos"

export interface Member {
    id: number
    username: string
    photoUrl: string
    age: number
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    introduction: string
    lookingFor: string
    city: string
    country: string
    photos: Photo[]
}  