import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/members';
import { map, of, tap } from 'rxjs';
import { Photo } from '../_models/photos';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private accountService = inject(AccountService);
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').subscribe({
      next: (members) => this.members.set((members)),
    });
  }
  // getMembers() {
  //   return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
  //         map((user) => {
  //           if(user.username)
  //           return user;
  //         })
  //       );
  // }

  getMember(username: string) {
    const member = this.members().find((x) => x.username === username);

    if (member !== undefined) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseUrl + 'users/delete-photo/' + photo.id)
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photos = m.photos.filter((x) => x.id !== photo.id);
              }
              return m;
            })
          );
        })
      );
  }
}
