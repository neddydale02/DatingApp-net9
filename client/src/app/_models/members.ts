import { Photo } from "./photos"

export interface Member {
    id: number
    userName: string
    age: number
    photoUrl: string
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    introduction: string
    interests: string
    myProperty: any
    lookingFor: string
    city: string
    country: string
    photos: Photo[]
  }