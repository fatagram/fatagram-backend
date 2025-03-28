//using Fatagram.Domain.Models;
//using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
//using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
//using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
//using System.Diagnostics;

//namespace Fatagram.Infrastructure.Repositories.AccountRepository
//{
//    /// <summary>
//    /// Mock account repository
//    /// </summary>
//    public class MAccountRepository : IAccountRepository
//    {
//        public List<Account> Accounts { get; set; }

//        private readonly IUserRepository _userRepository;
//        private readonly IUserPrivacyRepository _userPrivacyRepository;

//        public MAccountRepository(IUserRepository userRepository, IUserPrivacyRepository userPrivacyRepository)
//        {
//            _userRepository = userRepository;
//            _userPrivacyRepository = userPrivacyRepository;

//            Accounts = new List<Account>();

//            var newUser1 = new User()
//            {
//                Id = Guid.NewGuid(),
//                LastName = "Phat",
//                FirstName = "Ngoc",
//                FullName = "Ngoc Phat",
//                Email = "ngocphat123@gmail.com",
//                Avatar = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBxMTEhUTExMVFRUWGBsVGRgXGBgXGBgbGBgYFxobGBcYHiggGBomIBYaITEiJSkrLi4uGB8zODMtNygtLisBCgoKDg0OGhAQGi0lHyUtLS0tLS0tLS0tLS0tLS0tLS0tLS0rLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLf/AABEIAOkA2AMBIgACEQEDEQH/xAAcAAACAgMBAQAAAAAAAAAAAAAEBQADAQIGBwj/xABEEAACAQIDBQUFBQYFAwQDAAABAhEAAwQSITFBUWFxBSKBkaEGEzKxwUJSYtHwI3KCkrLhFDOi4vFTY8I0Q1STFRYX/8QAGQEAAwEBAQAAAAAAAAAAAAAAAAECAwQF/8QAJhEAAgICAgEEAwADAAAAAAAAAAECEQMhEjFBBBRRYRMiMkJiof/aAAwDAQACEQMRAD8A7mpUqVsakqVKlAEqVKlAEqVhmAEkwBvNBnGO3+UmYfeY5V8N5oGlYbUpFjr+UxexKofuICzfyjX0pLf7RAb9k98cWZHRf11EUGkcV+TscTiktiXdVHMxPTjSTGe1tldEDOf5R66+lJsRaLks7ZiftHbHhpHSKUvdtLpmUc93nTRvH08V/R1Nr2he4JD2LQ4OLhb5RVOK9pXtCTew90k/AgeY/eGg8a58EESCCOIMjzFa0y/wROkHtls/Y9e//tqy17Xie9ZYDirK3oYrlso4VUcKJ/LSikD9PD4PS8D2hbvCbbA8RsI6g6iiq8st3b1pg9ppjjo0cjsPQiu17C9ohdhLndc7DsDHhH2W5b91KjnyYHHa6H1SpUpGBKlSpQBKlSpQBKlSpQBKlSpQBKlSpQBKlSpQBK0vXQilmMAVvSW9jFe7J1VNVA+0Z0PiQfBedBUY2GMwg3b0Ko1CnYo3SN7H+1I+0e2XcwpNtOWlxup+wOQ15jZU7UxDNBbVvsgbF6Deee3pQtrB72qbOrHhXcigXo0VQg3kfEfH661LWLMwBIG0knyE7TWmIuAnKg0BieJ5dKJuIFXSg6KRo5LA6xzqm07oYOo/XkfQ8qsup3CBtjTrt+db22DKDuIB8xNKxlN3su1c7y9xj9pO6f4hsbxFLr9prZC3YgmFcfATuB/6bdZB404tGD+tf70U9pXUhgGVhBB1BHMUcqF0IPcnWNQNvFeo+tZKxtFEWsMbb+7JMxNpzrmA223O8jceHQ0daAI1HIj7p/Kq5jTFBWtSo2HfTe5gBu0pdisMV0PnVKSYwvsntq9YMEm7b+657w/dc/IyOlPsb7UWls57ffuE5RbOhB/HwUcRt3TNccjHYdo38aly2DyO4/rdVJKzDJhi9oddg+0V4XQmIcOlwwGChcjE6aDahmOI0512teUnWQdDy+YNeldkYr3ti3cO1kBP72xvUGqyJdo5MkaDKlSpWRmSpUqUASpUqUASpUqUASpUoLtXH+6SQMztoizEnmdyjaT9SBTBbKu28TlT3amHuaDiFkZ25QNJ4kUtVACSBqYB6DZ86qwGHZs91mzsTlZz9ojcg+yimQAN875JtcTp51E34O3FDiioLJzeX50P2jegZRtO2icVeywB8TaAfM9B+VBYiwFUsx2AkmoTNkAWFloG4T5kD5E0yxe0DxobA4fKVkd5kzHqWUx4CBRDrmuRwE/rzptjsxFU4IQscCV8ASB6UabdaJZieZmlYFZWirJ0nj860yVmzo0bjr4/r5UgZMdhBcQrMHarDarDUEeND4cl1zEAXF7rjdI2jodo6ij0O47R+gaGuJkuBxsfut1+yfp4jhQIll9366HmKl4Ke6d9bvbgzx+fH6eVa3LQOvnzpjFd7Bbt+78Q3jrw6UO9ort2HYd1O3tSIJ6HfQ9h9WtvE7RwIPDxnpNWpBYmupPXd+VOvZj2hW3ZNprdwlGaCAIIZi0SSAImh8T2aGBWSJ+E/wB+IpVgcSyubV0QwMHgeDDkZHSa3i1LTOfNG0dbd9pn+zZUc2cn0A+tBt7QYj7yDkE/MmgTWrLXQscfg5Ahu2cR/wBU+SD5LVb9p3j/AO6/gxHyoRqxNVwj8DGmG9oMQhALC4J2OBPgyxHiDUpYm0dRUqZY4/AUj0ipUqVxEmruACSYAEk8hXJdr33uvFuRcuQi/wDbQn+o7fD8Ip37QYkJbjaSR3QCS2uiiOLQPOhewcARdLP8arLD8b6STyUEAbhFV0rNcarYXftJZsrbXRVAA6D5k0vQQJO06n9cBTjGWM0D+Ly2evypHj1mLf3vi/dG3z0Hia5zqh0U4Nc5N0/a0Xkg2eJ2+XCp2jbzZU3Mwn90d4/KPGjQKHfVzxVdOrH/AG0iyu+ssrDgy+YDD+mt8Nb1Y9PrVty33YG6CPD9RViCiwNclYyVZSm92gy4r3R+FrQZf3gzkjxAPlQFjEpVOJTSRtXXyovnWuWgCi62gca8eYNb3LYZSNxHzquyIDJwOnQ6j5x4VMKY7viOm8eH1FAyize0Kt8Q0POKheNSe6dDyO4+Na9oWDmzDft6j+3yqvDNtRho2mtUVWgnNuobHYfOunxLqI28xVVyw6DuywXdvjivEcvKqreP3zs5H1qlFvom0XYHFh+4/wAW47M0bxwYcKA7fwskXNjLCtzBPdbwJj+I1L723JKMNTMAiVblGzjRAue9U23jPBAO5wRBB58fOtEmnZL2ivCXcyg76upZ2VdJkGZE6b8y6MPECfCmQNdsHZwzVFV5aooq4NKENaEltvaOoqVrb2iNsisVMmFnpdYJ31mqMZlClmAOUSJE6/qK88Qo997y7nGhMi2T9lRIa4Z5THNjwpj2IBkZxsdyVPFV7imd5OXN/FSbsvBFwXc9wGT+PLML+4Dt4md0zf2hjSiDLpAyqPCKU34OmMbdIOu47U5YjZ5UkTEZrjH+EdB/eauunJbPED12fOlVgRWR1RihwjSTy09J+tU4O5mLNxaB0AEVqzxaLbJn10+VadlnuA8vmf7CkFDDfWkwY/X6/MVvv8KxdSRzGz8qBG1Iu18MWuMy/GqIy9VZ2A8dnjTu20ir8Fgxc97pr3VB5gE/+VNCuuwLBXgygjYQGHQiR+XhVtCYG3kZrZ0gkjoxmPBp8CKIcww56Uhg+LfK6niI8tR8zVaPtO8d78x5VO1fhB+6ynwPdPzrTCnvCmUug9xI06iqXshhpod3I1vh9JXhs6HUfl4VqrQdd+nju8xQIXY/FHMq6oGE5hvI2oD9kjbxO7YaAwujOusAgjftGok8x608vYdXDW2Egw3AidZB3EEEzXP37b2rnf14PuYcDwblv2jeB1YJLowyp9kxuBVjOUHqBQ2CsObhVSSAubKTsIYDuk7NuzZpTYMCJ3baxg1C3GO8qADukk6TumBW2XUbM8bd0LMRmW6LwEEkC4NnfUaNH4lBU9BTRxBIGzaOhqzGBXGR/wBm20E7CRs6/rWqr7QLYOjL3SOR2MDvEgDxrPHPZeSGmaNcoZzrRjW6HdIrrTOQt7ItFr9tRvaegGpPpWKfeyGB+K8d/cToD3j5gD+E1muXNP8AbQHS0q7bDNCj4R3jz1AHgJ9RTWl+N/ybjnTTN0VTInynxrAqDp2V4NowtvmijzEmk+O71xF5g+s/Smqv+ytrwzA/wmPrQDL+1ngv9qiR14VSZpjlzZV4mT0H/NS1gwNavA73QD1J/IVY50/W+oNrBL2HN3JaGmbU9ANfrUsiNODR5a/WjOzbV1iGtBPgALPMCddANSdKCxVtku5HILEsSQIH2DoKYr3QaONa3GgE8CPpQ1vEaRvmrsP3lYc/oKQUU+9jWaYdldoRmVbbuSc3dyxsA2sRwpNjvgPT6U37Dwa3FYyVYEEMDBgjT5GgJpUV9rW3LLd9yyDY2qnQ6fZPQ+FAvdJAnaPpXS2ydVe7bdTpthvKTXN47D+7crMjceI3fl4UE43eiYsZ7ZI4Gfr+fhWtqzBHQGs4diCeG/lwP51eiZem7ly6cKCzJ2zVF06+OU9dqn1jxrdX41TeXvxudI6Mh0PXvf6aQzZwCVJ0Oq8CDt+nrVWJVoKuvvEO9dGHhv6jyqh8ZmtlhAdTlf8ACy744fQiqMD2w5VS6gyNcuhBGh0Oh1neK0jYmrFeKuNZJTVkYEo0RrvVhuP65Bv2MmdGY74XqAokEbxJagPaTFq5sKupNyTIggZWGzmT6Gs9hXblpFUtIZj8WgzFjpI+Gdx1BmNN+7cpQMElGY5uWwBlYZk4HUrzG8j1HyA7RwmVJEvbGvFk/Eh3jiPnTRb6scpBVuB0Pgdh8KFxjta7wGZPtDlx5H0PWsYt2aumgTDXg3CdvIjiPqN1XXUlT0oZ1Q62zzH647qYdkr7x0X8QLcgvePgYjxrvUqVnDJbOvwtgW0VBsUAeVSralcZBKrv2g6sh2MCp6EQfnVlSkAhsBsoz/EBDDdmGjEdY+VVx3z0HzNF4xh7xzsAAB4bM0+TDyoAYidd27nWTO+DtWXpv61hhMr+GR1BEVpYeTHj+v1uqsXggDMYjOnrI/p9aRbOg9nEHuFI3knyOUeiilnamHzX7kCSq5x0AQN6MaSj2oKL+w0UmIZd4ADETqPI7aFbt66p/wAS1u82uXuvAY6Ej4YjQaAcK04Mxum2HoQW03/MbfpRWFxCqGkxrPqFnpNXJ7nFWPe4ciQQYiGRt6Mu2NTSy7hX+7rsYHSRv/PrU0bRkpIv7SWFbhlLDpv8vyqX+0rVlXF1SykaouhImVO0aA6VRk0yg6b0bSJ0OXgeVHWuwWxGS4zrCDLly68Dm11PlQgnpbEFj2vRjltWEUcASdOegFb9pY91CtcssmYZlJBAI5ETxG2N1MMb7Nol4rJBKhpA0IkiNeH1ocdl3Hdlus3u7YVbRzAkgiSI3AGBXQsaas5fzUzXsvHgnNqZGwbdv96aspA2ab13jp+VIL3ZEAKpJNwtl3EZSkn5jwpv2bbItkEk6kSeEDZWE4pHTGV7Ngund1G8bx5/I1hyDlP3WnXTaCp+dU4PEC4ve+IDXcSOIIopEGySQRvM6VBoIO1LTWr5uJtcCQfhcfCVYeoO6eoOi2ws5fhmY3pO5uXA0y7UtSgO9GAPQ7/lSHHyGGXR2EKeBkDyhpI4CtYEvWzTDd+8G2yWI5BYC+sn+KnQQRBGh0ilOHcLdOneC7vhIJ4bj3d2nIVcMYc8KCTwAnz4V2R0jklbYV/inB93cC3FEEZpDEbNvEHfHDjWR2iyHKAzCJysQ2m/K+h8CDtobFy7JAIZGlpgCCNVO/WQfAU5TBK6Dcw1B4H6jcRwJrmnxTOiKdHO4cQSBOWZXkDrHhTr2fxwt4lSxAW4pRidIOhUzziPKhnwYzfdn7P3WGpHQ/EORNV3MJ3su2R6QZn0866VUo0cs1s9JqVyfs/2u1oizfJKMYt3GMwTAFtj8mPQ7plc0ouLpmJ1lSpQnaeL93bLDadF6n8ttSNK3SOex2KzMw/G0+BgeQAocvpWhNbWLee4lv7x16Db6TUHpKoxDey1MMx2lj4AAAD0nxqntG3c7wSNYYk7RGjQOg9aZ3RDv+8T56/WqUugqTu1HhvqemQnasS9mezSthRfUkuGh1A2AGGg79IPSj3wEC2iAG0WLOGJ2ZTBUcZiivZ7tdrWawLecgltsEiANOO7wYUbhsPezFrYS2rfYeXj93LEdJNbRyV2c+SErNPZfspbeIu3FGUFVEAmJ70yN+6uiv4JHMka8R9amEs5RzOpI0k9N1VPiW95A2TEVnOXJ2Qr8FT9iId59DV2DwPu5htDuijorBFSLnJ6bFHtBhWe3mSfeLqukzxB5Gkd98UbUDDIlwiJzBgOcGNa666KBxTQNk8Bxp82tFwSZzIzrJeALaBF3t3u+2Y7236Vue4FEbfSq8U5e6iAyoYs54kS3kDHyrPaphR1HzFDOqKrQgskrkcGCsT0Ohnlv8KcXnK6xIGumsfmp9PlRgsICDOzZRTAwBOuWAeYH68qGaM0fLcTMpkMI/LyNJmwPvApkghhBG0EggepFFYftIgw4EHaQNQeYG3rWDeIOkbQ2mqsJmRGo8JprQmhJiOzbrXgsP3oBZdFIBJOYjVTqd8Guo7OwKoogCtP/wAnbG1oPDb6DWicLfDbA0cSInoDrTlNtEqKQq7Stxeneyz1ymDP8y0d2fc0ih/aDQWnjY+U/usrT6haJ7PsgrO/aDypXopdFHbmBdgLlr/MTXL98DXL14dSN9UYK8Lii4NjDy4zO+flyp2pjSleIwnu7hdfguHvD7r7J5Bt/Pqa1wTp0znyx8m72wwysAQdCDvrFZuE91V1d2CKOZ3nkBLHkDUrplKK7OQ7Sue9o7vfVZ3SB5T81roa5XthC10vugqPBj9FrkZrg/oALU29nsLldXPxOmYclJIXzEHxNKEtF2VBtYhfA7T4CT4V1mMIR0eNPgPTd9aSOjNL/H5FXb1zKzAbWhvCIP8AT61rgDCLz/ufpQ/tE2Zy+6Mq9Br8yaIKxkA3GP8ASRUSKh/CQpTEC1fFz7KllP7pkRO4DQg8oOmzucBeR/hPgdD5GvPrqZ2KnZqW5yTA+vhXd+yWJF3DITGdJtPxzJ3SfEAN/FSaJzaQ5UVlE1JgcuPjWwFYa6ARoTPAE+ZGzxoONsuitGFai+SwGRoP2u7A8M0+lWkUElDCgMfYZhA0nfReLxQTaGPQfnSTFdtsZW2mvPVvIaDxPhRRtjT8AN7DBHCqZMQTwE6/IDz4UD2m2aFHEa88yiPWiLLFs06akE/aJHyGsVXejOo3KJ+vzA86DrRrhj8S/dMf6QfrQ2IxIBYb0AudVMgxz0b0rbAtJuc2n0FBYr/1KnkFPRgfrFFFg3aNvK0jY3eHjr8/nVlnCA2VdSQRMwd4JBMcefCsYZQwNreuq8xsI8KM7HPdZTuPoR+YNUxkwd8wJg+n621fg7s+M/M0ItvK0cDHhtH0qzCmGXqfWkxtF/ayzaadxU+TA0J2Tfyn3Z3fD02x+uBovtY/sX6D5iluJEDNsyka8JMA+BjzNJEofuoIqmxfX3gsXftg5Sdj8RyYaaeNYwGIzrzGhHA1r2nh8yhguZrbBwPvAfEvUiY4GDuoWmZy6GvZnZgt3GZjLAQkjYp2kfiJEHoONSi7FzMqw2aRmtv94ETB5x57doqVtd9nE1e0FCuX7SDZbcCe6XgbSBIIHOGPpXUUrxC5TlYb2NtuurKefDiOlJjxP9hL7Pw2IXfCsw8gAfJ66TtMj3TyAZECeJ0HrB8KSWptXg1tc2ZTpvgEFgvnI8d2wvFdoJdyhTESxUiGBiACD1OzgKm9GuSLlNC/tMSEHF1HkZPyrTE4v9ibqbhn14AyfSaq7VuaqOGY+SNFbdk/5WU/ZJSOW7/SRWZ00BM4nOPhbjtVvutw5cZrTA4u/hrjXrEPm/zLLGFuAbCD9hwNJ86Nw1gZWtn4kGWfvJrkmfiEaa7waCs2mG8AdD6S0CqRVKSpnf8As97RWMWp920XF+O0+lxDzXeOY0pzXkuO7PVyHVmS6vw3EOVx4jaOVPPZ322dVVMYpYbBftjhp+0tjUHmsjpVUcmTA47R34NSqMJikuqHturqdhUgj0pf2t7R4bD6XLgL/cTvuf4Rs8YqaOfi2NLqyIkjoYPnXP8Aa+NtWQyoAX3xrk5sfvHzpSntS2JDlT7lEbKyjW7yzH7IP4Z60IrhyAiHIDMkQCdus6njzNOjox42tsll8rIG+II7tyLsrR5z5Vqlww9w9B+vIfw1VitHYA7VYzrIPdEZvEbNmlb41ZyW136nkq/3I8qKOlGMARA46k+JI/8AH0obGWG96XCyBkk6aa8Kuw9sr3xsAgg71ZmII5jQ9CaKe2xJCn4gPDKTr6j0qlHYcjm3Yh8ymCGJB8T6a07wrgnONMwAYcDupPilGdwogBioH7vd9SCfGt7V5lIiNoBB2EHQzw2zPKhxLvVjfHW9jeBqnDfEOtEW7oMoZ4a7RyP0O+h8L8XQx5VA09F3aDzbP7yqf5h/zVKWg0q2qsCp6ERV+Mt/a3aT9D+vpVC0kJCzsy+1lxnOg7jk7wDGY8xt5gzXVrXOdq2O9mGxx/qUR6r/AEmjfZzFlreRvit93qv2T6R4c6b+SGOexyFZrJ+Ek3LfKTLKOYaWHU8KlaFJKmYIIg8NalUpHLkxu7Q+qnE4ZbghhOsjcQeIO41dUqjnEuKwVwDYWKnMrLAYEb4O/cdxBPGKXP2lm1IknkAPmaedt4rJaaD3m7i9Tv8AASfCuYw9rMwA2DbUS0duD9lbLksZhmO0n0IK/Wa0OHdGzJ3uKnSR9DzozEPlGlVJihGtQbg97GIdSTacSIcZZB2iTowPI7qoV5Onnu8ONYuYsXL1tN3ebrkG3+Yr5Vc4qkioqisvSw3sogCTqYmN52ndWMVayiZ1GnPl6ULkZTDb9Z48fEV0xgjKU2FwCJUsuYScrMhPJspE+NV2rCJEAAAyf5W2mtLWJAgNMxroTw4bqdey9yw2IQ3YZdQs/CLhjLmB8YneRyp8SZTSVlfYgb3oKAZ3BWNpKgFtY4QTy8TT/EYu1lgLda5+8oPkpgRzXrXbUp7WfDEEsiXXGxVAZz5axU1Zy+45Po410jLMsxgEDU6HORz12nnuozD4BiSzwM20DXQbFnhtnjJons/FYZM3vHtW7rnZmUKg3L3ZA5zHjFVY03Xn3bqibTcidPwz8XkBwJpxgvJcs16RTlT3rq+mxlgkEg7YjbqYrW5Zde8AQAZEmWA3g7e6fMVLfZjW4uKzPcmTnaSQd3Lw49KPuvMKPtCTyX8zs8+FaKCJ/IzncD2U1xnZmgZiQYEsCxbZuImJ2GJqDsoMzJbNxspytcZlCA7wAq98jhpHGnWMuqmi5Vdpg7AoG1zugT5kDfS8dtW7YCWkZlXfIAPEydWO+Y1prGgeVmzYMqwt3H94CpKkjKy5SoIzDbOb031X7spcABmRIzb40Oo8N2+tVxq3ro092SMssFadZATcDt27dKl629plFyWRW7tzkRGV+BBiDvAG+sMmJo2xZb0yy7iCLsEdwqJ5ElhPTTWtrtjLs2bqpxDj3xHG2pHgz/nRVh9IOysDoQHjVm0eKHOPDb6Ejxpdhr/u7wf7IbI37rhdfAwek0+azB4g6f8ANJLOGOUqw26Hyy/IUIOzq1rFUdnXCyITtgT1Gh9alIzZ0dSsExqdAKV47G50KWyQGEZ+R25OJ57Otat0cMYuT0I8fjDib5Fs9y2CqnaJPxOeOwAdDR9q0FED+5rXD2EtqFUQB6/maz70TG+sm7O6KpUZIpF2hoxGz6DeabPigATu2DmeVIscDdzDdHe8tF+p5daImsQvstQWkbAunjFE3lqrsjZPFRVt96fkryLMVhySBGhbXXXXbp0rOLtAiDxnp0pgLUDMdu7l/egb5raMrJpbF9ux33jcq/NzVGLWBM5SQQD4bDx6UdhW/angV/pP+/0rHa9y2tsq4nNIC7Sx5fnurrj0cWR1Kh7h/aO57pFuILugCkwSCdzFto5jXTftoTFl7vx3GI+4vdT+UbfGa57smw9xFzyCmm0zsBnTeQRT7D2AuyfEk/OqUV2c/QHf7PJhF1zstsRt77BdPOvST2ahYIG0QAkafwj0nypD7LYKWOKb4LYYJp8TQQzDkNVHMtwFdD2VhGWblzRju66maznLZLYrvdn3nLlGyorFREZjl0JiOIO8Uru4N0lzeiNT3PijQA96TwiukN5T75C5SD70EGDlPeOvVW8K5pLrXO+5Yz8IJOziRxPpThbE50LbnZZusbt5pcwFX7KAbO7Jlt889KDv4coYP/NdDVV+0GEGtloy5u9nNusiKbdiYyT7u5JLCAWYnMNpUgmJ57x6h4rDlT9aEdZ0/XhzolG0bQkGJDKpBMDMAd+XMShnoSPGiLeIjRv5t3jw+VKuzXIOR7hGoC91cpB04TpI04Twpi1efOFM9TFJSQwW+V12rvB+hq67h1cSND5HxpVhpzKg1Un+UDX+UwBymn5SsXob0yvCrkQA7tvnNSq8Qe73gSvIww105GpRRLLsRca4xLSF2Km4RvYbCx9NOpxNKcf22EOvc1gZgxJ8Ij51U9x21JuR0KD0ihlRikqQ2v3go7xA/W4UtfEzooMc9J/IUMq8Br+tpraxaLasZXcNgPM8aVF0kbMzOdOk7l6celX4LA5iLa7zqd8faY8/rFTQDlTLs/Ei0DlGe62jfdtgfZZt54gb9DsmmiMk2lrsWSECjdkWANSduwb6IsYYzmbwXh14n5VZh7fwnfkUTVrvAmkVYLjW3UmvvrA1J2fmeAo3G39tKrmJNs5yJB2jeAJMjw3VvjQpuokUm0zE94kKBwG0mB5dat7J7LfE3soGpBZp3KN07pMAAcZ50QpRwGHezEKANpYmAscZOyu+9l+xlsBztdoDH93aByBJHhO+uuUlFHm5JnAYYEXb4OkXNnCEVfmpovssLfxFuzrkZjmIMTlUsVB3DTXyoTtBSzPcUke8ZmMcCxYfOsdhYsWcTh2Pw+9VP/sm2PVhVv8Akg9QuIJS0oAVYcgaAKvwjlLR4IaoxeIzEgEAASSdij7zfQf3qtbxJYxGY7d+UaKOWmvUmud7YUi4xILTBUalZOgHBTp5CemEYkA+MK3rwCZslsHM5/8AcLQQs7xoCd2wbDRRrFi1lETJ2k8SdprJFbpUZSdmprU1sa1JqiAfGKChnhSnFYYqeW402xjd3qQPMit71oMINMuMqOWvbVEbWEHgRqPrR7XIcg7GiPIfkfKq8TZIcLw73gP7kVQjyzIeAI6SdnQ/SuXMrZ6np3SHXZSzcJ+6v9R/2inLbKSezrk+8J2lv6Rl+hp4K4pdm77F/aOiAcWUf6gfkKladuXADZB33P8Axb86lNDQ8FhL10XHIKWiQgJEF/tP4fCP4uVA4hlJOoiTvFfPvZuAe+/u0jNldwDOvu0a4QIGrEKYG8wKNv8As3ilVGFl3DqjAorNHvYKKYHxkMhj/uJxqW7PPh6jiuj157QBbURs28dT6aeNYe6BzO4DUn8up0ryQ+yuNhT/AIa93na2BkbNKKrnuxMZXBnYdeFW4D2RxVxWdk9wix37+a2pJYqAGKwNVILGFWNSKRr7x/B6xZwhYhrjZQDIRW8izDaeQ060xUqBAgAbAIivDsV7NYu2CWw92Ai3SwUlVR1zgsRoNOOyDwrN32ZxSW2d7LplIlWVleCrvmgj4ALbSdxEUE+6+j2s31UASNABt4CgcXigBJI5V4XUpplL1n+v/T1+7eHxMYA3c/qeVCXBmYBtrbp+FBBI6kwD/avK6laxy14Jl6rkqo989i7CNimc5f2VvMJj43OVT1hXHjXfG+iKBmB8RrXyLUpyzW7o5W7PpfG9mYVz3QbROncK5fFDI8opQnskBdGbEKRbuW7miROVg4UnP+EbONfP9SqXqGOz6b7YxZCEW2CnSW0JGZsojntPhzpJgXzalu6hYLJkkknM5J1JiFHRuNfP9SheorwS9n0Z7wcR5isFxxHnXzpUqvdfRPE+iC44jzqgXBmOo00214JewtxPjR11K95SNREjXeJGnMVqbDCJVtYI0Os7I4zT939C4HvGLYEoJHxDfwk/Sr844jzrwTFYG7aMXLboeDqyncdhH4h5jjVT2WESpEiRIIkSRI46gjwNHu/oOB7T2i4LMZGi5fqfn6UluHupcGrIAY4qR3h5a9QK8uynhRw7ExMBhh7xUiQRbcgiASQYiII86h+ovwdKzUkkuj2Lsm8sSCIJJ8yW+RpuuI0mQ3UgHz2H0rwduwsUInDXxJgTafUkTA026HyqHsPFf/Gv7Af8p9jfCdm/dWDdmvuvo94L23ZQzAA5kO4gspA6ajQ8YrFfPuJw722KXEZHG1WBVhv1B1FShSM5Z23aN+zsY1m7bvJ8VtldZ2SpBE8tK6X/APoWK1GW3GYkAe8CqjMGNoIHym3plEgsBsYVyVSpMDoV9q2yNa/w+HNpplP20ZStgZJF3NE4Wy0zmldsEglJ7eYj3/8AiDbstd0gxcWCHe5IKXFJBNwyhJRoWVMCuUqUAdLZ9s7yhctuyGRcqPlfMh9yuHLgZ8uY20RSCpXuggAyTMR7Z3Wt3bQs2Ft3tbiAXCGM3GzSzkgh7mcQdCi7pB5qpQBKlSpQBKlSpQBKlSpQBKlSpQBKlSpQBKlSpQB0tj22xKoEGQgKibGBOTeSGBLHedugIg61s3t1iojujRhP7SQGEHa/ACDtGv3mnmKlAHVL7e4kHRbQg5gIeAcoU6F+9MEnNMkztCxQ/tjfJtkrbJthwhm7mAufF3xczE/iJnnXOVKAOo//AHrFQAcuWIYDMMwJUme8QPgiIgAkAa1VZ9sLyFiqWxmZn1DmGZswIBaAVhQIGoRZnKI5ypQB0GF9rr6ABVt6KbY0cEWyiIUBDAgfs1Ob4pHxamrLvtnfYEMlshtoPvCDIykFc8RGgWIXaoB1rm6lABXaWOa9ca6wAZomJ3ALtYkk6akkkmSSSalC1KAP/9k=",
//                Background = "https://static.wikia.nocookie.net/unanything/images/8/8b/Average_Anime_Girl.png/revision/latest?cb=20230326002111"
//            };
//            Debug.WriteLine("User 1: " + newUser1.Id.ToString());


//            var newAccount1 = new Account()
//            {
//                Id = Guid.NewGuid(),
//                Username = "ngocphat123",
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456789"),
//                UserId = newUser1.Id,
//                User = newUser1
//            };

//            _userRepository.CreateUserAsync(newUser1);
//            CreateAccountAsync(newAccount1);

//            var newUser2 = new User()
//            {
//                Id = Guid.NewGuid(),
//                LastName = "Tran",
//                FirstName = "Minh",
//                FullName = "Minh Tran",
//                Email = "minhtran@mail.com"
//            };
//            Debug.WriteLine("User 2: " + newUser2.Id.ToString());

//            var newAccount2 = new Account()
//            {
//                Id = Guid.NewGuid(),
//                Username = "minhtran",
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456@ppt"),
//                UserId = newUser2.Id,
//                User = newUser2
//            };

//            _userRepository.CreateUserAsync(newUser2);
//            CreateAccountAsync(newAccount2);
//        }

//        public Task<bool> CreateAccountAsync(Account newAccount)
//        {
//            Accounts.Add(newAccount);
//            return Task.FromResult(true);
//        }

//        public Task<Account?> GetAccountByUsernameAsync(string username)
//        {
//            return Task.FromResult(Accounts.FirstOrDefault(account => account.Username == username));
//        }

//        public Task<Account?> GetAccountByIdAsync(Guid accountId)
//        {
//            return Task.FromResult(Accounts.FirstOrDefault(account => account.Id == accountId));
//        }

//        public Task<Account?> GetAccountByIdAsync(string accountId) => GetAccountByIdAsync(Guid.Parse(accountId));


//        public Task<Account?> GetAccountByUserIdAsync(Guid userId)
//        {
//            return Task.FromResult(Accounts.FirstOrDefault(account => account.UserId == userId));
//        }

//        public Task<bool> UpdateAccountAsync(Account account)
//        {
//            var index = Accounts.FindIndex(a => a.Id == account.Id);
//            if (index == -1)
//            {
//                return Task.FromResult(false);
//            }
//            Accounts[index] = account;
//            return Task.FromResult(true);
//        }

//        public Task<Account?> GetAccountByUserIdAsync(string userId) => GetAccountByUserIdAsync(Guid.Parse(userId));


//    }
//}
