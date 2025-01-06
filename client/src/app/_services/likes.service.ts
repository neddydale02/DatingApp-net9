import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  constructor() {
    this.loadLikes();
  }

  toggleLike(targetId: number) {
    const liked = this.likeIds().includes(targetId);
    if (liked) {
      this.likeIds.set(this.likeIds().filter(id => id !== targetId));
    } else {
      this.likeIds.set([...this.likeIds(), targetId]);
    }
    this.saveLikes();
    return this.http.post(`${this.baseUrl}likes/${targetId}`, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = setPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return this.http.get<Member[]>(`${this.baseUrl}likes`, {
      observe: 'response', params}).subscribe({
      next: response => setPaginatedResponse(response, this.paginatedResult)
    });
  }

  getLikeIds() {
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next: ids => {
        if (JSON.stringify(ids) !== JSON.stringify(this.likeIds())) {
          this.likeIds.set(ids);
          this.saveLikes();
        }
      },
      error: err => console.error('Failed to fetch like IDs', err)
    });
  }

  private saveLikes() {
    localStorage.setItem('likedMembers', JSON.stringify(this.likeIds()));
  }

  private loadLikes() {
    const storedLikes = localStorage.getItem('likedMembers');
    if (storedLikes) {
      this.likeIds.set(JSON.parse(storedLikes));
    }
  }
}
