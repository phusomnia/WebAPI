mapfile -t dbcontexts < <(dotnet ef dbcontext list | grep -E '^[A-Za-z0-9_.]+$')

echo "Select a DbContext: "
select choice in "${dbcontexts[@]}"; do
    if [[ -n "$choice" ]]; then
        dotnet ef migrations add InitialCreate --context $choice 
        dotnet ef database update --context $choice
        break
    else
        echo "Invalid selection. Try again."
    fi
done