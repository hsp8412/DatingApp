import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of, map, Observable, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { getPaginationHeaders, getpaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;
  // paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
          this.userParams = new UserParams(this.user);
        }
      },
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
    }
  }

  getMembers(userParams: UserParams) {
    const key = Object.values(userParams).join('-');
    const cache = this.memberCache.get(key);
    if (cache) return of(cache);
    let params = getPaginationHeaders(
      userParams.pageSize,
      userParams.pageNumber
    );

    params = params.append('gender', userParams.gender);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('minAge', userParams.minAge);
    params = params.append('orderBy', userParams.orderBy);

    const url = this.baseUrl + 'users';

    return getpaginatedResult<Member[]>(url, params, this.http).pipe(
      map((members) => {
        this.memberCache.set(Object.values(userParams).join('-'), members);
        return members;
      })
    );
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, curr) => arr.concat(curr.result), [])
      .find((m: Member) => m.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageSize, pageNumber);
    params = params.append('predicate', predicate);
    return getpaginatedResult<Member[]>(
      this.baseUrl + 'likes',
      params,
      this.http
    );
  }
}
