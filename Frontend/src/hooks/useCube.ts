import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
import { Face, Colour } from '../types/api'

type ExplodedView = (Colour | null)[][]

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export function useCube() {
  const queryClient = useQueryClient()

  const explodedQuery = useQuery<ExplodedView, Error>({
    queryKey: ['cube', 'exploded'],
    queryFn: async () => {
      const { data } = await axios.get<ExplodedView>(
        `${API_BASE}/api/rubiksCube/exploded`
      )
      return data
    },
    staleTime: 30_000,
    refetchOnWindowFocus: false,
  })

  const resetMutation = useMutation<void, Error, void>({
    mutationFn: async () => {
      await axios.post<void>(`${API_BASE}/api/rubiksCube/reset`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cube', 'exploded'] })
    },
  })

  const moveMutation = useMutation<
    void,               // TData
    Error,              // TError
    { face: Face; clockwise: boolean } // TVariables
  >({
    mutationFn: async ({ face, clockwise }) => {
      await axios.post<void>(`${API_BASE}/api/rubiksCube/move`, { face, clockwise });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cube', 'exploded'] })
    },
  })

  return {
    // query state
    exploded:      explodedQuery.data,
    isLoadingView: explodedQuery.isLoading,
    isErrorView:   explodedQuery.isError,

    // reset mutation
    resetCube:     resetMutation.mutate,
    isResetting:   resetMutation.isPending,

    // move mutation
    moveCube:      moveMutation.mutate,
    isMoving:      moveMutation.isPending,
  }
}
